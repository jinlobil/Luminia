@echo off
setlocal
set "LOGDIR=%USERPROFILE%\AppData\LocalLow\SoloDeveloper\Luminia\Logs"

if not exist "%LOGDIR%" (
  echo No Luminia error log folder exists yet.
  echo Start the game once, then try again.
  pause
  exit /b 1
)

start "Luminia Logs" "%LOGDIR%"
