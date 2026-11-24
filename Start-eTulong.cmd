@echo off
setlocal
cd /d "%~dp0"
call "%~dp0settings.example.cmd"
if exist "%~dp0settings.local.cmd" call "%~dp0settings.local.cmd"
echo Checking PostgreSQL schema. Existing cases are preserved.
"%~dp0publish\Api\Api.exe" --initialize
if errorlevel 1 (
 echo Start PostgreSQL and check settings.local.cmd. See README.md.
 pause
 exit /b 1
)
powershell -NoProfile -Command "try{$r=Invoke-RestMethod 'http://127.0.0.1:5291/health' -TimeoutSec 2;if($r.status -eq 'ready'){exit 0};exit 1}catch{exit 1}"
if errorlevel 1 start "eTulong API - close after demo" /min "%~dp0publish\Api\Api.exe" --urls http://127.0.0.1:5291
powershell -NoProfile -Command "for($i=0;$i -lt 30;$i++){try{$r=Invoke-RestMethod 'http://127.0.0.1:5291/health' -TimeoutSec 1;if($r.status -eq 'ready'){exit 0}}catch{};Start-Sleep -Milliseconds 300};exit 1"
if errorlevel 1 (
 echo API unavailable. Inspect its console and check port5291.
 pause
 exit /b 1
)
start "eTulong Desk" /wait "%~dp0publish\Desktop\eTulong.Desktop.exe"
