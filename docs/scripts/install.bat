@echo off
setlocal enabledelayedexpansion

for /F "tokens=2* skip=2" %%a in ('reg query "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders" /v "personal"') do set DocumentsFolder=%%b

call set "DocumentsFolder=!DocumentsFolder!"
set InstallationPath=%DocumentsFolder%\Klei\OxygenNotIncluded\mods\Local\MultiplayerMod\

echo "Installing multiplayer mod in folder: %InstallationPath%"
xcopy /s .\MultiplayerMod "%InstallationPath%"
pause
