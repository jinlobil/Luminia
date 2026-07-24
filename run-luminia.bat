@echo off
setlocal
cd /d "%~dp0"

echo ============================================================
echo                       LUMINIA
echo ============================================================
echo This window will build the game automatically when needed.
echo Keep this window open until the game starts.
echo.

powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0Scripts\Run-Luminia.ps1"
set "RESULT=%ERRORLEVEL%"

if not "%RESULT%"=="0" (
  echo.
  echo Luminia could not start. Error code: %RESULT%
  echo Please send Build\unity-build.log and a screenshot of this window.
  echo.
  pause
)

exit /b %RESULT%
