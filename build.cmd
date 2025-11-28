@echo off
REM This script builds, tests, and runs the Kennel Management System project.

REM --- Build ---
echo "Building & testing the project..."
dotnet test
if %errorlevel% neq 0 exit /b %errorlevel%

REM --- Run ---
echo "Running the project..."
dotnet run --project src/kms.csproj
