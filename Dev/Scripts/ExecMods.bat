@ECHO off

SET "SCRIPT_FOLDER=%~dp0"
SHIFT /1 & SET "FUNCTION_NAME=%~1"

CD "%SCRIPT_FOLDER%\..\..\"

FOR /f usebackq %%a IN (`DIR "Eco.RM.*" /b/a:d`) DO (
	CALL :ExecMod %%a
)

GOTO :Close

:ExecMod
CD "%SCRIPT_FOLDER%"
ECHO Executing %FUNCTION_NAME% On %~1
CALL ModFuncLib.bat %FUNCTION_NAME% %~1
EXIT /b 0

:Close
PAUSE
EXIT /b 0

