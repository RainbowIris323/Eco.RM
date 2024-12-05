@ECHO OFF

SET "mod_file_name=%1"

SET "asset=%cd%\..\Builds\Assets\%mod_file_name%.unity3d"
SET "source=%cd%\..\%mod_file_name%\bin\Debug\net8.0\%mod_file_name%.dll"
SET "staging=%cd%\..\Builds\Staging\Mods\RaynboMods\%mod_file_name%"
MD "%staging%"
MD "%staging%\Assets"

COPY "%source%" "%staging%\%mod_file_name%.dll"
COPY "%asset%" "%staging%\Assets\%mod_file_name%.unity3d"
exit 0