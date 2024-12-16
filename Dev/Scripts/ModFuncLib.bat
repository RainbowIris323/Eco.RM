@ECHO off
CD "%~dp0"
SHIFT /1 & GOTO :%~1
GOTO :Return



:Build
SET MOD_NAME=%~1

ECHO "Starting Mod Build For: %MOD_NAME%"
SET "MOD_DIR=%cd%\..\..\%MOD_NAME%"
SET "MOD_DLL_FILE=%MOD_DIR%\bin\Debug\net8.0\%MOD_NAME%.dll"
SET "OUTPUT_FILE=%cd%\..\Temp\%MOD_NAME%.dll"

ECHO "Building..."
dotnet build "%MOD_DIR%" >NUL 2>&1

ECHO "Moving Output..."
COPY "%MOD_DLL_FILE%" "%OUTPUT_FILE%" /y >NUL 2>&1

ECHO "Cleaning Up Files..."
RMDIR /s/q "%MOD_DIR%\bin" >NUL 2>&1
RMDIR /s/q "%MOD_DIR%\obj" >NUL 2>&1
ECHO "Finsished Building Mod: %MOD_NAME%"
GOTO :Return



:CreatePreRelease
CALL :Build %~1
SET MOD_NAME=%~1

ECHO "Creating Pre-Release For Mod: %MOD_NAME%"
SET "MOD_DIR=%cd%\..\..\%MOD_NAME%"
SET "STAGE_DIR=%cd%\..\Temp\%MOD_NAME% (Stage)"
SET "MOD_DLL_FILE=%cd%\..\Temp\%MOD_NAME%.dll"
SET "MOD_ASSET_DIR=%cd%\..\AssetBundles\%MOD_NAME%"
SET "MOD_DATA_DIR=%cd%\..\ModData\%MOD_NAME%"

ECHO "Verifing Directorys..."
MD "%MOD_ASSET_DIR%" >NUL 2>&1

if NOT EXIST "%MOD_DATA_DIR%\" (
	ECHO "Creating Mod Data..."
	XCOPY "%cd%\..\Templates\ModData" "%MOD_DATA_DIR%" /E /I /H /C /Y >NUL 2>&1
)

SET /p MOD_VERSION=< "%MOD_DATA_DIR%\Version"

ECHO "Creating Stage Directorys..."
XCOPY "%cd%\..\Templates\ModStage" "%STAGE_DIR%" /E /I /H /C /Y >NUL 2>&1
MD "%STAGE_DIR%\Mods\Raynbo Mods\%MOD_NAME%" >NUL 2>&1

ECHO "Adding DLL File To Stage..."
COPY "%MOD_DLL_FILE%" "%STAGE_DIR%\Mods\Raynbo Mods\%MOD_NAME%\%MOD_NAME%.dll" /y >NUL 2>&1

ECHO "Adding Asset Bundles To Stage..."
XCOPY "%MOD_ASSET_DIR%" "%STAGE_DIR%\Mods\Raynbo Mods\%MOD_NAME%\Assets" /E /I /H /C /Y >NUL 2>&1

IF EXIST "%MOD_DIR%\UserCode\" (
	ECHO "Adding UserCode To Stage..."
	XCOPY "%MOD_DIR%\UserCode" "%STAGE_DIR%\Mods\UserCode" /E /I /H /C /Y >NUL 2>&1
)

ECHO "Packaging Stage As Pre-Release..."
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "Compress-Archive -Update '%STAGE_DIR%\Mods' '%MOD_DATA_DIR%\Packed\%MOD_NAME% v%MOD_VERSION% (Pre-Release).zip'" >NUL 2>&1

ECHO "Cleaning Up Files..."
DEL /F/Q/S "%MOD_DLL_FILE%" >NUL 2>&1
RMDIR /s/q "%STAGE_DIR%" >NUL 2>&1

ECHO "Finished Creating Pre-Release For Mod: %MOD_NAME%"
GOTO :Return



:LoadPreRelease
SET MOD_NAME=%~1
SET "MOD_DATA_DIR=%cd%\..\ModData\%MOD_NAME%"

ECHO "Loading Pre-Release On Servers"

if NOT EXIST "%MOD_DATA_DIR%\" (
	ECHO "Creating Mod Data..."
	XCOPY "%cd%\..\Templates\ModData" "%MOD_DATA_DIR%" /E /I /H /C /Y >NUL 2>&1
)

SET /p MOD_VERSION=< "%MOD_DATA_DIR%\Version"
SET "PACKED_MOD_FILE=%MOD_DATA_DIR%\Packed\%MOD_NAME% v%MOD_VERSION% (Pre-Release).zip"

IF NOT EXIST "%PACKED_MOD_FILE%" (
	CALL :CreatePreRelease %~1
)

SET /p DEV_SERVER_DIR=< "%cd%\..\Configs\DevServerDir"
SET /p TEST_SERVER_DIR=< "%cd%\..\Configs\TestServerDir"

ECHO "Loading %MOD_NAME% On The Test Server"
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "Expand-Archive -Force '%PACKED_MOD_FILE%' '%TEST_SERVER_DIR%'" >NUL 2>&1
ECHO "Finished Loading %MOD_NAME% On The Test Server"

ECHO "Loading %MOD_NAME% On The Development Server"
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "Expand-Archive -Force '%PACKED_MOD_FILE%' '%DEV_SERVER_DIR%'" >NUL 2>&1
ECHO "Finished Loading %MOD_NAME% On The Development Server"

ECHO "Finished Loading Pre-Release On Servers"

GOTO :Return



:CreateAndLoadPreRelease

CALL :CreatePreRelease %~1
CALL :LoadPreRelease %~1

GOTO :Return



:BootServer
SET /p TEST_SERVER_DIR=< "%cd%\..\Configs\TestServerDir"

tasklist /fi "ImageName eq EcoServer.exe" /fo csv 2>NUL | find /I "EcoServer.exe">NUL
IF "%ERRORLEVEL%"=="0"  (
	ECHO "A Server Is Already Running!!"
) ELSE (
	ECHO "Booting Test Server"
	CD "%TEST_SERVER_DIR%"
	CMD /K "EcoServer.exe"
	ECHO "Test Server Process Ended"
)

PAUSE
GOTO :Return



:CreateChangelog
SET MOD_NAME=%~1
SET "MOD_DATA_DIR=%cd%\..\ModData\%MOD_NAME%"
SET /p MOD_VERSION=< "%MOD_DATA_DIR%\Version"
SET "MOD_CHANGE_LOG_DIR=%MOD_DATA_DIR%\Changelogs"
SET "VERSION_CHANGE_LOG_FILE=%MOD_CHANGE_LOG_DIR%\%MOD_VERSION%.md"

IF NOT EXIST "%cd%\..\..\%MOD_NAME%\Changes" (
	ECHO None>"%cd%\..\..\%MOD_NAME%\Changes"
)

ECHO "Please Verify All Changes Are Included."
START "" /wait notepad.exe "%cd%\..\..\%MOD_NAME%\Changes"

ECHO ## Version #%MOD_VERSION%>"%VERSION_CHANGE_LOG_FILE%"

FOR /F "tokens=* delims=,"  %%a IN ('TYPE "%cd%\..\..\%MOD_NAME%\Changes"') DO (
    ECHO - %%a>>"%VERSION_CHANGE_LOG_FILE%"
)

SET "MOD_CHANGE_LOG_FILE=%MOD_DATA_DIR%\Changelog.md"

ECHO # %MOD_NAME% Changelog>"%MOD_CHANGE_LOG_FILE%"

FOR /L %%a in (%MOD_VERSION%,-1,1) do (
    FOR /F "tokens=* delims=,"  %%b IN ('TYPE "%MOD_CHANGE_LOG_DIR%\%%a.md"') DO (
        ECHO %%b>>"%MOD_CHANGE_LOG_FILE%"
    )
)

GOTO :Return



:CreateRelease
SET /p "MOD_NAME=Mod Name: "
SET "MOD_DATA_DIR=%cd%\..\ModData\%MOD_NAME%"
SET /p MOD_VERSION=< "%MOD_DATA_DIR%\Version"
SET "VERSION_CHANGE_LOG_FILE=%MOD_DATA_DIR%\Changelogs\%MOD_VERSION%.md"

if NOT EXIST "%MOD_DATA_DIR%\" (
	ECHO "Creating Mod Data..."
	XCOPY "%cd%\..\Templates\ModData" "%MOD_DATA_DIR%" /E /I /H /C /Y >NUL 2>&1
)

IF NOT EXIST "%PACKED_MOD_FILE%" (
	CALL :CreatePreRelease %MOD_NAME%
)

ECHO "Moving Pre-Release To Release"
COPY "%MOD_DATA_DIR%\Packed\%MOD_NAME% v%MOD_VERSION% (Pre-Release).zip" "%MOD_DATA_DIR%\Packed\%MOD_NAME% v%MOD_VERSION% (Release).zip" /Y >NUL 2>&1
DEL /F/Q/S "%MOD_DATA_DIR%\Packed\%MOD_NAME% v%MOD_VERSION% (Pre-Release).zip" >NUL 2>&1

ECHO "Finalizing Changelog"
CALL :CreateChangelog %MOD_NAME%
ECHO None>"%cd%\..\..\%MOD_NAME%\Changes"

ECHO "Bumping Pre-Release Version"
SET /a "NEW_VERSION=%MOD_VERSION%+1"
>"%MOD_DATA_DIR%\Version" ECHO %NEW_VERSION%

PAUSE
GOTO :Return



:Return
EXIT /b 0