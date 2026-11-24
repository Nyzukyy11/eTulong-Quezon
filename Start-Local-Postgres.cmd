@echo off
setlocal
rem Convenience helper for this existing workspace, not a PostgreSQL installer.
for %%I in ("%~dp0..\..\work\toolchain") do set "TOOLS=%%~fI"
if not exist "%TOOLS%\postgres\pgsql\bin\postgres.exe" (
 echo Start your PostgreSQL service and configure the database as described in README.md.
 pause
 exit /b 1
)
echo Keep this PostgreSQL console open during the local demo.
"%TOOLS%\postgres\pgsql\bin\postgres.exe" -D "%TOOLS%\pgdata" -h 127.0.0.1 -p 55432
pause
