@ECHO off

SET "RUNNER=%a"

ECHO **************************************************
ECHO *                Making Mod Files                *
ECHO **************************************************

SET "SOLUTION_FOLDER=%cd%\..\.."
SET "STORAGE_FOLDER=%SOLUTION_FOLDER%\Storage"
SET "ASSET_BUNDLES_FOLDER=%STORAGE_FOLDER%\AssetBundles"

SET TEST_SERVER_ENABLED=1
SET "TEST_SERVER=C:\Program Files (x86)\Steam\steamapps\common\Eco\Eco_Data\Server"
SET DEV_SERVER_ENABLED=1
SET "DEV_SERVER=C:\Eco Modding\Source Code\Eco.CodeOnly\Server"

CALL :MAKE_MOD Eco.RM.ElectricTakeover, 1.0.2
CALL :MAKE_MOD Eco.RM.WorldPeace, 0.0.0
CALL :MAKE_MOD Eco.RM.Framework, 1.0.0

CALL :CLONE_CONFIGS

ECHO *                 Made Mod Files                 *
ECHO **************************************************

EXIT 0

:CLONE_CONFIGS
IF %TEST_SERVER_ENABLED% == 0 ( 
	IF %DEV_SERVER_ENABLED% == 0 (
		EXIT /B 0
	)
)

IF %TEST_SERVER_ENABLED% == 1 (
	ECHO Cloning Configs To The Test Server
	XCOPY "%STORAGE_FOLDER%\Configs" "%TEST_SERVER%\Configs" /E /I /H /C /Y >NUL 2>&1
	ECHO Cloned Configs To The Test Server
)

IF %DEV_SERVER_ENABLED% == 1 (
	ECHO Cloning Configs To The Development Server
	XCOPY "%STORAGE_FOLDER%\Configs" "%DEV_SERVER%\Configs" /E /I /H /C /Y >NUL 2>&1
	ECHO Cloned Configs To The Development Server
)

ECHO **************************************************
EXIT /B 0

:MAKE_MOD
SET "MOD_NAME=%~1"
SET "MOD_VERSION=%~2"
SET "MOD_BUILD_FILE_NAME=%MOD_NAME%-v%MOD_VERSION%.zip"

SET "MOD_BUILD_FOLDER=%STORAGE_FOLDER%\%MOD_NAME%"
SET "MOD_PACKED_FOLDER=%MOD_BUILD_FOLDER%\Packed"
SET "MOD_STAGE_FOLDER=%MOD_BUILD_FOLDER%\Staging\Mods\RaynboMods\%MOD_NAME%"
SET "MOD_BUILD_FILE=%MOD_PACKED_FOLDER%\%MOD_BUILD_FILE_NAME%"

SET "MOD_USERCODE_FOLDER=%SOLUTION_FOLDER%\%MOD_NAME%\UserCode"
SET "MOD_CODE_FILE=%SOLUTION_FOLDER%\%MOD_NAME%\bin\Debug\net8.0\%MOD_NAME%.dll"
SET "MOD_ASSET_FOLDER=%ASSET_BUNDLES_FOLDER%\%MOD_NAME%"

ECHO Making Mod: %MOD_NAME% Version %MOD_VERSION%

ECHO Creating Missing Mod Directorys
MD "%MOD_PACKED_FOLDER%" >NUL 2>&1
MD "%MOD_ASSET_FOLDER%" >NUL 2>&1
MD "%MOD_USERCODE_FOLDER%" >NUL 2>&1

ECHO Removing Old Files
DEL /F/Q/S "%MOD_STAGE_FOLDER%" >NUL 2>&1
DEL /F/Q/S "%MOD_BUILD_FILE%" >NUL 2>&1

ECHO Creating Staging Directory
MD "%MOD_STAGE_FOLDER%" >NUL 2>&1

ECHO Cloning Assets
XCOPY "%MOD_ASSET_FOLDER%" "%MOD_STAGE_FOLDER%\Assets" /E /I /H /C /Y >NUL 2>&1

ECHO Cloning DLLs
COPY "%MOD_CODE_FILE%" "%MOD_STAGE_FOLDER%\%MOD_NAME%.dll" >NUL 2>&1

ECHO Cloning UserCode
XCOPY "%MOD_USERCODE_FOLDER%" "%MOD_BUILD_FOLDER%\Staging\Mods\UserCode" /E /I /H /C /Y >NUL 2>&1

ECHO Finalizing Archive
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "Compress-Archive '%MOD_BUILD_FOLDER%\Staging\Mods' '%MOD_BUILD_FILE%'" >NUL 2>&1

ECHO Finished Making Mod: %MOD_NAME% Version %MOD_VERSION%
ECHO **************************************************

IF %TEST_SERVER_ENABLED% == 0 ( 
	IF %DEV_SERVER_ENABLED% == 0 (
		EXIT /B 0
	)
)

IF %TEST_SERVER_ENABLED% == 1 (
	ECHO Setting Up %MOD_NAME% On The Test Server
	PowerShell -NoProfile -ExecutionPolicy Bypass -Command "Expand-Archive -Force '%MOD_BUILD_FILE%' '%TEST_SERVER%'" >NUL 2>&1
	ECHO Finished Setting Up %MOD_NAME% On The Test Server
)

IF %DEV_SERVER_ENABLED% == 1 (
	ECHO Setting Up %MOD_NAME% On The Development Server
	PowerShell -NoProfile -ExecutionPolicy Bypass -Command "Expand-Archive -Force '%MOD_BUILD_FILE%' '%DEV_SERVER%'" >NUL 2>&1
	ECHO Finished Setting Up %MOD_NAME% On The Development Server
)

ECHO **************************************************
EXIT /B 0