@echo off
setlocal
cd /d "%~dp0"

if exist "Build\Windows\Luminia.exe" (
  start "Luminia" "Build\Windows\Luminia.exe"
  exit /b 0
)

echo ============================================================
echo Luminia executable was not found.
echo.
echo 1. Open this folder in Unity Hub with Unity 6000.0.35f1.
echo 2. In Unity, choose: Luminia ^> Build Windows
echo 3. Run this file again.
echo.
echo Full instructions: README.md
echo ============================================================
pause
exit /b 1
