# ============================================
# NDTCore.Identity - Migration Utilities
# Run from: NDTCore.Identity.API
# ============================================

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# -------------------------------
# Project configuration
# -------------------------------
$ProjectName = "NDTCore.Identity.Infrastructure"
$StartupProject = $PWD.Path
$ProjectPath = Join-Path $PWD.Path ("..\" + $ProjectName)
$Context = "NdtCoreIdentityDbContext"
$MigrationsBase = Join-Path $ProjectPath "Persistence\Migrations"
$MigrationsDir = Join-Path $MigrationsBase "AspNetIdentityDb"

# -------------------------------
# Validate project structure
# -------------------------------
function Test-ProjectSetup {
    if (-not (Test-Path $ProjectPath)) {
        Write-Host ""
        Write-Host "Error: Infrastructure project not found at:" -ForegroundColor Red
        Write-Host $ProjectPath -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Please run this script from NDTCore.Identity.API folder" -ForegroundColor Red
        Read-Host "Press Enter to exit"
        exit 1
    }
    
    # Create migrations directory if it doesn't exist
    if (-not (Test-Path $MigrationsDir)) {
        New-Item -Path $MigrationsDir -ItemType Directory -Force | Out-Null
    }
}

# -------------------------------
# Helper: Run EF Command
# -------------------------------
function Invoke-EfCommand {
    param(
        [string]$Command,
        [switch]$NoErrorCheck
    )
    
    Write-Host ""
    Write-Host "Running: dotnet ef $Command" -ForegroundColor Cyan
    Write-Host ""
    
    try {
        $fullCommand = "ef $Command --context $Context --project `"$ProjectPath`" --startup-project `"$StartupProject`""
        $output = Invoke-Expression "dotnet $fullCommand" 2>&1
        
        if ($LASTEXITCODE -ne 0 -and -not $NoErrorCheck) {
            Write-Host ""
            Write-Host "❌ Command failed with exit code: $LASTEXITCODE" -ForegroundColor Red
            Write-Host $output -ForegroundColor Red
            return $false
        }
        
        Write-Host $output
        return $true
    }
    catch {
        Write-Host ""
        Write-Host "❌ Error executing command:" -ForegroundColor Red
        Write-Host $_.Exception.Message -ForegroundColor Red
        return $false
    }
}

# -------------------------------
# Menu Display
# -------------------------------
function Show-Menu {
    Clear-Host
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "NDTCore.Identity - Migration Utilities" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Context: " -NoNewline -ForegroundColor Gray
    Write-Host $Context -ForegroundColor White
    Write-Host "Project: " -NoNewline -ForegroundColor Gray
    Write-Host $ProjectName -ForegroundColor White
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host " [Info & Status]" -ForegroundColor Yellow
    Write-Host "  1. View Migration Status" -ForegroundColor White
    Write-Host "  2. List All Migrations" -ForegroundColor White
    Write-Host ""
    Write-Host " [Create & Update]" -ForegroundColor Yellow
    Write-Host "  3. Create New Migration" -ForegroundColor White
    Write-Host "  4. Update Database (Latest)" -ForegroundColor White
    Write-Host "  5. Update to Specific Migration" -ForegroundColor White
    Write-Host "  6. Generate SQL Script Only" -ForegroundColor White
    Write-Host ""
    Write-Host " [Maintenance]" -ForegroundColor Yellow
    Write-Host "  7. Remove Last Migration" -ForegroundColor White
    Write-Host "  8. Refresh Migration (Remove + Create)" -ForegroundColor White
    Write-Host ""
    Write-Host " [Danger Zone]" -ForegroundColor Red
    Write-Host "  9. Rollback All Migrations" -ForegroundColor Red
    Write-Host " 10. Drop Database" -ForegroundColor Red
    Write-Host ""
    Write-Host "  0. Exit" -ForegroundColor Gray
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
}

# -------------------------------
# Menu Functions
# -------------------------------
function Show-MigrationStatus {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "Migration Status" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    
    $success = Invoke-EfCommand "migrations list --no-build"
    
    if ($success) {
        Write-Host ""
        Write-Host "✅ Status retrieved successfully" -ForegroundColor Green
    }
    
    Write-Host ""
    Read-Host "Press Enter to continue"
}

function Show-Migrations {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "All Migrations" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    
    $success = Invoke-EfCommand "migrations list --no-build"
    
    if ($success) {
        Write-Host ""
        Write-Host "✅ Migrations listed successfully" -ForegroundColor Green
    }
    
    Write-Host ""
    Read-Host "Press Enter to continue"
}

function New-CustomMigration {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "Create New Migration" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host ""
    
    $migrationName = Read-Host "Enter migration name"
    
    if ([string]::IsNullOrWhiteSpace($migrationName)) {
        Write-Host "Migration name cannot be empty" -ForegroundColor Yellow
        Read-Host "Press Enter to continue"
        return
    }
    
    # Clean migration name (remove spaces, special chars)
    $migrationName = $migrationName -replace '[^a-zA-Z0-9_]', ''
    
    Write-Host ""
    Write-Host "Creating migration: $migrationName" -ForegroundColor Yellow
    
    $success = Invoke-EfCommand "migrations add $migrationName --output-dir Persistence/Migrations/AspNetIdentityDb"
    
    if ($success) {
        Write-Host ""
        Write-Host "✅ Migration created successfully" -ForegroundColor Green
        Write-Host ""
        
        $apply = Read-Host "Apply migration to database now? (y/n)"
        
        if ($apply -eq 'y') {
            Write-Host ""
            Write-Host "Applying migration to database..." -ForegroundColor Yellow
            
            $updateSuccess = Invoke-EfCommand "database update"
            
            if ($updateSuccess) {
                Write-Host ""
                Write-Host "✅ Database updated successfully" -ForegroundColor Green
            }
        }
    }
    
    Write-Host ""
    Read-Host "Press Enter to continue"
}

function Update-DatabaseToLatest {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "Update Database to Latest" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host ""
    
    $confirm = Read-Host "Update database to latest migration? (y/n)"
    
    if ($confirm -ne 'y') {
        return
    }
    
    Write-Host ""
    Write-Host "Updating database..." -ForegroundColor Yellow
    
    $success = Invoke-EfCommand "database update"
    
    if ($success) {
        Write-Host ""
        Write-Host "✅ Database updated successfully" -ForegroundColor Green
    }
    
    Write-Host ""
    Read-Host "Press Enter to continue"
}

function Update-ToSpecificMigration {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "Update to Specific Migration" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Available migrations:" -ForegroundColor Yellow
    Write-Host ""
    
    Invoke-EfCommand "migrations list --no-build" | Out-Null
    
    Write-Host ""
    $migrationName = Read-Host "Enter migration name (or '0' to revert all)"
    
    if ([string]::IsNullOrWhiteSpace($migrationName)) {
        Write-Host "Operation cancelled" -ForegroundColor Yellow
        Read-Host "Press Enter to continue"
        return
    }
    
    Write-Host ""
    Write-Host "Updating to migration: $migrationName" -ForegroundColor Yellow
    
    $success = Invoke-EfCommand "database update $migrationName"
    
    if ($success) {
        Write-Host ""
        Write-Host "✅ Database updated successfully" -ForegroundColor Green
    }
    
    Write-Host ""
    Read-Host "Press Enter to continue"
}

function New-SqlScript {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "Generate SQL Script" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host ""
    
    $outputFile = Read-Host "Enter output filename (default: migration.sql)"
    
    if ([string]::IsNullOrWhiteSpace($outputFile)) {
        $outputFile = "migration.sql"
    }
    
    # Add .sql extension if not present
    if (-not $outputFile.EndsWith(".sql")) {
        $outputFile += ".sql"
    }
    
    $fullPath = Join-Path $StartupProject $outputFile
    
    Write-Host ""
    Write-Host "Generating SQL script..." -ForegroundColor Yellow
    Write-Host "Output: $fullPath" -ForegroundColor Gray
    
    $success = Invoke-EfCommand "migrations script --output `"$fullPath`" --idempotent"
    
    if ($success) {
        Write-Host ""
        Write-Host "✅ SQL script generated successfully" -ForegroundColor Green
        Write-Host "Location: $fullPath" -ForegroundColor Cyan
    }
    
    Write-Host ""
    Read-Host "Press Enter to continue"
}

function Remove-LastMigration {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "Remove Last Migration" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "⚠️  WARNING: This will remove the most recent migration" -ForegroundColor Yellow
    Write-Host ""
    
    $confirm = Read-Host "Remove the last migration? (y/n)"
    
    if ($confirm -ne 'y') {
        Write-Host "Operation cancelled" -ForegroundColor Yellow
        Read-Host "Press Enter to continue"
        return
    }
    
    Write-Host ""
    Write-Host "Removing last migration..." -ForegroundColor Yellow
    
    $success = Invoke-EfCommand "migrations remove --force"
    
    if ($success) {
        Write-Host ""
        Write-Host "✅ Last migration removed successfully" -ForegroundColor Green
    }
    
    Write-Host ""
    Read-Host "Press Enter to continue"
}

function Refresh-Migration {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "Refresh Migration" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "This will:" -ForegroundColor Yellow
    Write-Host "1. Remove the last migration" -ForegroundColor White
    Write-Host "2. Create a new migration with updated changes" -ForegroundColor White
    Write-Host ""
    
    $confirm = Read-Host "Continue? (y/n)"
    
    if ($confirm -ne 'y') {
        Write-Host "Operation cancelled" -ForegroundColor Yellow
        Read-Host "Press Enter to continue"
        return
    }
    
    # Step 1: Remove last migration
    Write-Host ""
    Write-Host "[Step 1/2] Removing last migration..." -ForegroundColor Yellow
    
    $removeSuccess = Invoke-EfCommand "migrations remove --force"
    
    if (-not $removeSuccess) {
        Write-Host ""
        Write-Host "Failed to remove last migration. Aborting refresh." -ForegroundColor Red
        Read-Host "Press Enter to continue"
        return
    }
    
    # Step 2: Create new migration
    Write-Host ""
    Write-Host "[Step 2/2] Creating new migration..." -ForegroundColor Yellow
    Write-Host ""
    
    $migrationName = Read-Host "Enter new migration name (default: NdtCoreIdentityMigration)"
    
    if ([string]::IsNullOrWhiteSpace($migrationName)) {
        $migrationName = "NdtCoreIdentityMigration"
    }
    
    Write-Host ""
    $success = Invoke-EfCommand "migrations add $migrationName --output-dir Persistence/Migrations/AspNetIdentityDb"
    
    if ($success) {
        Write-Host ""
        Write-Host "✅ Migration refreshed successfully" -ForegroundColor Green
    }
    
    Write-Host ""
    Read-Host "Press Enter to continue"
}

function Rollback-AllMigrations {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Red
    Write-Host "DANGER: Rollback All Migrations" -ForegroundColor Red
    Write-Host "============================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "⚠️  WARNING: This will revert ALL migrations" -ForegroundColor Yellow
    Write-Host "⚠️  All database tables will be dropped" -ForegroundColor Yellow
    Write-Host ""
    
    $confirm1 = Read-Host "Type 'ROLLBACK' to confirm"
    
    if ($confirm1 -ne 'ROLLBACK') {
        Write-Host "Operation cancelled" -ForegroundColor Green
        Read-Host "Press Enter to continue"
        return
    }
    
    Write-Host ""
    Write-Host "Rolling back all migrations..." -ForegroundColor Yellow
    
    $success = Invoke-EfCommand "database update 0"
    
    if ($success) {
        Write-Host ""
        Write-Host "✅ All migrations rolled back successfully" -ForegroundColor Green
    }
    
    Write-Host ""
    Read-Host "Press Enter to continue"
}

function Remove-Database {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Red
    Write-Host "DANGER: Drop Database" -ForegroundColor Red
    Write-Host "============================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "⚠️  CRITICAL WARNING: This will DELETE the entire database" -ForegroundColor Yellow
    Write-Host "⚠️  ALL DATA WILL BE PERMANENTLY LOST" -ForegroundColor Yellow
    Write-Host ""
    
    $confirm1 = Read-Host "Type 'DELETE' to confirm database deletion"
    
    if ($confirm1 -ne 'DELETE') {
        Write-Host "Operation cancelled" -ForegroundColor Green
        Read-Host "Press Enter to continue"
        return
    }
    
    Write-Host ""
    Write-Host "Dropping database..." -ForegroundColor Yellow
    
    $success = Invoke-EfCommand "database drop --force"
    
    if ($success) {
        Write-Host ""
        Write-Host "✅ Database dropped successfully" -ForegroundColor Green
    }
    
    Write-Host ""
    Read-Host "Press Enter to continue"
}

# -------------------------------
# Main Program
# -------------------------------
try {
    # Validate setup
    Test-ProjectSetup
    
    # Main loop
    do {
        Show-Menu
        $choice = Read-Host "Select option (0-10)"
        
        switch ($choice) {
            '1'  { Show-MigrationStatus }
            '2'  { Show-Migrations }
            '3'  { New-CustomMigration }
            '4'  { Update-DatabaseToLatest }
            '5'  { Update-ToSpecificMigration }
            '6'  { New-SqlScript }
            '7'  { Remove-LastMigration }
            '8'  { Refresh-Migration }
            '9'  { Rollback-AllMigrations }
            '10' { Remove-Database }
            '0'  { 
                Write-Host ""
                Write-Host "Exiting..." -ForegroundColor Gray
                exit 0
            }
            default { 
                Write-Host ""
                Write-Host "Invalid option. Please select 0-10" -ForegroundColor Red
                Start-Sleep -Seconds 1
            }
        }
    } while ($true)
}
catch {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Red
    Write-Host "Unexpected Error" -ForegroundColor Red
    Write-Host "============================================" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}