@echo off

FOR /F "tokens=2* skip=2" %%a in ('reg query "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders" /v "personal"') do set DocumentsFolder=%%b

echo "Installing multiplayer mod in folder: %DocumentsFolder%\Klei\OxygenNotIncluded\mods\Local\MultiplayerMod\"
xcopy /s .\MultiplayerMod %DocumentsFolder%\Klei\OxygenNotIncluded\mods\Local\MultiplayerMod\
pause
