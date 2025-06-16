@echo off
setlocal enabledelayedexpansion

REM Check if mongodump is installed and on PATH
mongodump --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: mongodump not found. Please install MongoDB Database Tools and add them to your PATH.
    echo Download from: https://www.mongodb.com/try/download/database-tools
    pause
    exit /b 1
)

REM Check for at least 1 argument (computer name)
if "%~1"=="" (
    echo Usage: %~nx0 COMPUTER_NAME [TIMESTAMP]
    echo.
    echo Example: %~nx0 Dave-PC 2020-01-01-01-01-01
    echo          %~nx0 Dave-PC
    exit /b 1
)

REM Arguments
set "COMPUTER_NAME=%~1"
set "TIMESTAMP=%~2"

REM Script directory
set "SCRIPT_DIR=%~dp0"

REM Path to appsettings.json
set "JSON_FILE=%SCRIPT_DIR%appsettings.json"

REM Extract DefaultConnection from JSON via PowerShell
for /f "usebackq delims=" %%A in (`powershell -NoProfile -Command "(Get-Content -Raw '%JSON_FILE%' | ConvertFrom-Json).MongoConnectionStrings.DefaultConnection"`) do (
    set "CONNECTION_STRING=%%A"
)

REM Base path to Database\ComputerName
set "BASE_DIR=%SCRIPT_DIR%Database\%COMPUTER_NAME%"

REM If TIMESTAMP not provided, find the latest folder inside BASE_DIR
if "%TIMESTAMP%"=="" (
    set "LATEST="
    for /f "delims=" %%D in ('dir /b /ad /o:-n "%BASE_DIR%" 2^>nul') do (
        if not defined LATEST (
            set "LATEST=%%D"
        )
    )
    if not defined LATEST (
        echo ERROR: No backup folders found for computer "%COMPUTER_NAME%" in "%BASE_DIR%"
        exit /b 2
    )
    set "TIMESTAMP=!LATEST!"
)

REM Full path to restore folder
set "RESTORE_DIR=%BASE_DIR%\%TIMESTAMP%"

REM Find the single database folder inside the timestamp folder
set "DB_SUBDIR="

for /f "delims=" %%D in ('dir /b /ad "%RESTORE_DIR%"') do (
    if defined DB_SUBDIR (
        echo ERROR: More than one database folder found inside "%RESTORE_DIR%".
        exit /b 5
    )
    set "DB_SUBDIR=%%D"
)

if not defined DB_SUBDIR (
    echo ERROR: No database folder found inside "%RESTORE_DIR%".
    exit /b 6
)

set "RESTORE_DB_DIR=%RESTORE_DIR%\%DB_SUBDIR%"

REM Check folder exists
if not exist "%RESTORE_DB_DIR%" (
    echo ERROR: Restore folder "%RESTORE_DB_DIR%" does not exist.
    exit /b 3
)

echo Restoring MongoDB from:
echo    Computer Name: %COMPUTER_NAME%
echo    Timestamp:     %TIMESTAMP%
echo    Folder:        %RESTORE_DB_DIR%
echo.

REM Run mongorestore
mongorestore --uri="%CONNECTION_STRING%" --drop "%RESTORE_DB_DIR%"

if errorlevel 1 (
    echo ERROR: mongorestore failed.
    exit /b 4
)

echo.
echo MongoDB restore complete!
pause