::rmdir /S /Q "NDTCore.Identity.Infrastructure/Persistence/Migrations"

::dotnet ef migrations add NdtCoreIdentityMigration -c NdtCoreIdentityDbContext -o Persistence/Migrations/AspNetIdentityDb --project ../NDTCore.Identity.Infrastructure
::dotnet ef migrations add PersistedGrantMigration -c PersistedGrantDbContext -o Persistence/Migrations/PersistedGrantDb --project ../NDTCore.Identity.Infrastructure
::dotnet ef migrations add IdentityServerConfigMigration -c ConfigurationDbContext -o Persistence/Migrations/ConfigurationDb --project ../NDTCore.Identity.Infrastructure

::dotnet ef migrations script -c NdtCoreIdentityDbContext -o ../NDTCore.Identity.Infrastructure/Persistence/Migrations/AspNetIdentityDb.sql
::dotnet ef migrations script -c PersistedGrantDbContext -o ../NDTCore.Identity.Infrastructure/Persistence/Migrations/PersistedGrantDb.sql
::dotnet ef migrations script -c ConfigurationDbContext -o ../NDTCore.Identity.Infrastructure/Persistence/Migrations/ConfigurationDb.sql

::# Using Package Manager Console (PMC) #

::Add-Migration NdtCoreIdentityMigration -Context NdtCoreIdentityDbContext -OutputDir Persistence/Migrations/AspNetIdentityDb -Project NDTCore.Identity.Infrastructure
::Add-Migration PersistedGrantMigration -Context PersistedGrantDbContext -OutputDir Persistence/Migrations/PersistedGrantDb -Project NDTCore.Identity.Infrastructure
::Add-Migration IdentityServerConfigMigration -Context ConfigurationDbContext -OutputDir Persistence/Migrations/ConfigurationDb -Project NDTCore.Identity.Infrastructure

::Script-Migration -Context NdtCoreIdentityDbContext -Output ./NDTCore.Identity.Infrastructure/Persistence/Migrations/AspNetIdentityDb.sql
::Script-Migration -Context PersistedGrantDbContext -Output ./NDTCore.Identity.Infrastructure/Persistence/Migrations/PersistedGrantDb.sql
::Script-Migration -Context ConfigurationDbContext -Output ./NDTCore.Identity.Infrastructure/Persistence/Migrations/ConfigurationDb.sql

::Update-Database -Context NdtCoreIdentityDbContext
::Update-Database -Context PersistedGrantDbContext
::Update-Database -Context ConfigurationDbContext

@echo off
setlocal enabledelayedexpansion

:: Set project variables
set PROJECT_NAME=NDTCore.Identity.Infrastructure
set STARTUP_PROJECT=%CD%  :: The script runs inside the startup project
set PROJECT_PATH=%CD%\..\%PROJECT_NAME%
set MIGRATIONS_PATH=%PROJECT_PATH%\Persistence\Migrations\AspNetIdentityDb

echo -----------------------------------
echo Starting EF Core Migrations...
echo -----------------------------------

:: Ensure Migrations folder exists, clearing if necessary
if exist "%MIGRATIONS_PATH%" (
    rd /s /q "%MIGRATIONS_PATH%"
)

mkdir "%MIGRATIONS_PATH%"

:: Run EF Core migration commands
echo Creating Initial Identity Database Migration...
dotnet ef migrations add NdtCoreIdentityMigration -c NdtCoreIdentityDbContext -o Persistence/Migrations/AspNetIdentityDb --project %PROJECT_PATH%

::echo Creating Persisted Grant Migration...
::dotnet ef migrations add PersistedGrantMigration -c PersistedGrantDbContext -o Persistence/Migrations/PersistedGrantDb --project %PROJECT_PATH%

::echo Creating Identity Server Configuration Migration...
::dotnet ef migrations add IdentityServerConfigMigration -c ConfigurationDbContext -o Persistence/Migrations/ConfigurationDb --project %PROJECT_PATH%

:: Generate SQL scripts from migrations
echo Generating SQL Scripts...
dotnet ef migrations script -c NdtCoreIdentityDbContext -o %PROJECT_PATH%/Persistence/Migrations/AspNetIdentityDb/AspNetIdentityDb.sql
::dotnet ef migrations script -c PersistedGrantDbContext -o %PROJECT_PATH%/Persistence/Migrations/PersistedGrantDb.sql
::dotnet ef migrations script -c ConfigurationDbContext -o %PROJECT_PATH%/Persistence/Migrations/ConfigurationDb.sql

::Update database
echo Updating Database...
dotnet ef database update -c NdtCoreIdentityDbContext
::dotnet ef database update -c PersistedGrantDbContext
::dotnet ef database update -c ConfigurationDbContext

echo -----------------------------------
echo EF Core Migrations Completed!
echo -----------------------------------

endlocal
