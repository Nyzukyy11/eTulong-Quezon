@echo off
rem Copy to settings.local.cmd for private credentials; do not submit that local file.
set "ETULONG_DB=Host=127.0.0.1;Port=55432;Database=etulong_demo_1113;Username=postgres"
set "ETULONG_API=http://127.0.0.1:5291/"
set "ASPNETCORE_URLS=http://127.0.0.1:5291"
