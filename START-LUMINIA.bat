@echo off
setlocal
cd /d "%~dp0"

echo ============================================================
echo             LUMINIA WINDOWS LAUNCHER v2
echo ============================================================
echo.

if not exist "%~dp0Scripts\Run-Luminia.ps1" (
  echo [ERROR] The automatic launcher file is missing.
  echo.
  echo This is an incomplete or old copy of the project.
  echo Download the latest project ZIP again and extract ALL files.
  echo Do not copy only this BAT file into an older project folder.
  echo.
  pause
  exit /b 20
)

powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0Scripts\Run-Luminia.ps1"
set "RESULT=%ERRORLEVEL%"

if not "%RESULT%"=="0" (
  echo.
  echo [ERROR] Luminia could not start. Error code: %RESULT%
  echo Send a screenshot of this window and Build\unity-build.log.
  echo.
  pause
)

exit /b %RESULT%
