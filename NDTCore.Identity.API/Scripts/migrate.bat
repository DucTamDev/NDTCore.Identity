@echo off
setlocal enabledelayedexpansion

:: ============================================
:: NDTCore.Identity - EF Core Migration Script
:: Run from: NDTCore.Identity.API
:: ============================================

:: Set project variables
set PROJECT_NAME=NDTCore.Identity.Infrastructure
set STARTUP_PROJECT=%CD%
set PROJECT_PATH=%CD%\..\%PROJECT_NAME%
set MIGRATIONS_BASE=%PROJECT_PATH%\Persistence\Migrations
set IDENTITY_MIGRATIONS=%MIGRATIONS_BASE%\NdtCoreIdentityDb

echo ============================================
echo NDTCore.Identity - EF Core Migrations
echo ============================================
echo Startup Project: %STARTUP_PROJECT%
echo Infrastructure: %PROJECT_PATH%
echo ============================================

:: Check if project exists
if not exist "%PROJECT_PATH%" (
    echo Error: Project not found at %PROJECT_PATH%
    echo Please run this script from NDTCore.Identity.API folder
    goto :error
)

:: ============================================
:: Step 1: Clean Migrations
:: ============================================
echo.
echo [Step 1/4] Cleaning existing migrations...
if exist "%IDENTITY_MIGRATIONS%" (
    echo Removing NdtCoreIdentityDb migrations...
    rd /s /q "%IDENTITY_MIGRATIONS%"
    echo - Cleaned successfully
) else (
    echo - No existing migrations found
)

echo Creating migrations directory...
mkdir "%IDENTITY_MIGRATIONS%" 2>nul

:: ============================================
:: Step 2: Create Migration
:: ============================================
echo.
echo [Step 2/4] Creating new migration...
echo.
echo Migration: NdtCoreIdentityMigration
echo Context: NdtCoreIdentityDbContext
echo Output: Persistence/Migrations/NdtCoreIdentityDb
echo.

dotnet ef migrations add NdtCoreIdentityMigration ^
    --context NdtCoreIdentityDbContext ^
    --project "%PROJECT_PATH%" ^
    --startup-project "%STARTUP_PROJECT%" ^
    --output-dir Persistence/Migrations/NdtCoreIdentityDb ^
    --verbose

if errorlevel 1 (
    echo.
    echo Error: Failed to create migration
    goto :error
)

echo - Migration created successfully

:: ============================================
:: Step 3: Generate SQL Script
:: ============================================
echo.
echo [Step 3/4] Generating SQL script...
echo.

dotnet ef migrations script ^
    --context NdtCoreIdentityDbContext ^
    --project "%PROJECT_PATH%" ^
    --startup-project "%STARTUP_PROJECT%" ^
    --output "%IDENTITY_MIGRATIONS%\NdtCoreIdentityDb.sql" ^
    --idempotent ^
    --verbose

if errorlevel 1 (
    echo.
    echo Error: Failed to generate SQL script
    goto :error
)

echo - SQL script generated: NdtCoreIdentityDb.sql

:: ============================================
:: Step 4: Update Database
:: ============================================
echo.
echo [Step 4/4] Updating database...
echo.

dotnet ef database update ^
    --context NdtCoreIdentityDbContext ^
    --project "%PROJECT_PATH%" ^
    --startup-project "%STARTUP_PROJECT%" ^
    --verbose

if errorlevel 1 (
    echo.
    echo Error: Failed to update database
    goto :error
)

echo - Database updated successfully

:: ============================================
:: Success Summary
:: ============================================
echo.
echo ============================================
echo ✓ Migration Completed Successfully!
echo ============================================
echo.
echo Generated Files:
echo - Migration Classes: %IDENTITY_MIGRATIONS%\*.cs
echo - SQL Script: %IDENTITY_MIGRATIONS%\NdtCoreIdentityDb.sql
echo.
echo Database: Updated and ready to use
echo ============================================
goto :end

:: ============================================
:: Error Handler
:: ============================================
:error
echo.
echo ============================================
echo ✗ Migration Failed!
echo ============================================
echo.
echo Troubleshooting:
echo 1. Ensure you're in NDTCore.Identity.API folder
echo 2. Check connection string in appsettings.json
echo 3. Verify SQL Server is running
echo 4. Install EF Core tools: dotnet tool install --global dotnet-ef
echo 5. Check that NuGet packages are restored
echo.
echo Connection String Location:
echo %STARTUP_PROJECT%\appsettings.json
echo.
echo ============================================
pause
exit /b 1

:end
echo.
pause
endlocal