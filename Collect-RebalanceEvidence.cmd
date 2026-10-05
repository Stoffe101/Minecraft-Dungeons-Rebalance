@echo off
cd /d "%~dp0"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\Collect-RebalanceEvidence.ps1"
set "collector_result=%errorlevel%"
echo.
if not "%collector_result%"=="0" echo Collection recorded issues. Send the output ZIP anyway so partial evidence can be reviewed.
echo Send the rebalance-evidence ZIP from the .research folder back to this chat.
pause
exit /b %collector_result%
