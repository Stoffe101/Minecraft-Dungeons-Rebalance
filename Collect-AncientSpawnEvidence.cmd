@echo off
cd /d "%~dp0"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\Collect-RebalanceEvidence.ps1" -TargetSet AncientSpawn
set "collector_result=%errorlevel%"
echo.
if not "%collector_result%"=="0" echo Collection recorded issues. Send the ZIP anyway so partial evidence can be reviewed.
echo Send the new rebalance-ancient-evidence ZIP from the .research folder back to this chat.
pause
exit /b %collector_result%
