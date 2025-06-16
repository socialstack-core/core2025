@echo off
setlocal

REM Check if mongodump is installed and on PATH
mongodump --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: mongodump not found. Please install MongoDB Database Tools and add them to your PATH.
    echo Download from: https://www.mongodb.com/try/download/database-tools
    pause
    exit /b 1
)

REM Set script directory
set "SCRIPT_DIR=%~dp0"

REM Set path to appsettings.json
set "JSON_FILE=%SCRIPT_DIR%appsettings.json"

REM Extract DefaultConnection from JSON using PowerShell (on one line)
for /f "usebackq delims=" %%A in (`powershell -NoProfile -Command "(Get-Content -Raw '%JSON_FILE%' | ConvertFrom-Json).MongoConnectionStrings.DefaultConnection"`) do (
    set "CONNECTION_STRING=%%A"
)

REM Get computer name
set "COMPUTER_NAME=%COMPUTERNAME%"

REM Get ISO-formatted datetime (replace ':' with '-' for Windows compatibility)
for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format yyyy-MM-dd-HH-mm-ss"') do set "TIMESTAMP=%%i"

REM Build full output path
set "OUTPUT_DIR=%SCRIPT_DIR%Database\%COMPUTER_NAME%\%TIMESTAMP%"
mkdir "%OUTPUT_DIR%" >nul 2>&1

REM Run mongodump with connection string
mongodump --uri="%CONNECTION_STRING%" --out="%OUTPUT_DIR%"

echo.
echo Dump saved to: %OUTPUT_DIR%
pause