# ============================================
# NDTCore.Identity - EF Core Migration Script
# Run from: NDTCore.Identity.API
# ============================================

# Enable strict mode
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Set project variables
$ProjectName = "NDTCore.Identity.Infrastructure"
$StartupProject = $PWD.Path
$ProjectPath = Join-Path $PWD.Path ("..\" + $ProjectName)
$MigrationsBase = Join-Path $ProjectPath "Persistence\Migrations"
$IdentityMigrations = Join-Path $MigrationsBase "AspNetIdentityDb"

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "NDTCore.Identity - EF Core Migrations" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Startup Project: $StartupProject"
Write-Host "Infrastructure: $ProjectPath"
Write-Host "============================================" -ForegroundColor Cyan

# Check if project exists
if (-not (Test-Path $ProjectPath)) {
    Write-Host ""
    Write-Host "Error: Project not found at $ProjectPath" -ForegroundColor Red
    Write-Host "Please run this script from NDTCore.Identity.API folder" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

# ============================================
# Step 1: Clean Migrations
# ============================================
Write-Host ""
Write-Host "[Step 1/4] Cleaning existing migrations..." -ForegroundColor Yellow

if (Test-Path $IdentityMigrations) {
    Write-Host "Removing AspNetIdentityDb migrations..."
    Remove-Item -Path $IdentityMigrations -Recurse -Force
    Write-Host "- Cleaned successfully" -ForegroundColor Green
} else {
    Write-Host "- No existing migrations found" -ForegroundColor Gray
}

Write-Host "Creating migrations directory..."
New-Item -Path $IdentityMigrations -ItemType Directory -Force | Out-Null

# ============================================
# Step 2: Create Migration
# ============================================
Write-Host ""
Write-Host "[Step 2/4] Creating new migration..." -ForegroundColor Yellow
Write-Host ""
Write-Host "Migration: NdtCoreIdentityMigration"
Write-Host "Context: IdentityDbContext"
Write-Host "Output: Persistence/Migrations/AspNetIdentityDb"
Write-Host ""

try {
    dotnet ef migrations add NdtCoreIdentityMigration `
        --context IdentityDbContext `
        --project $ProjectPath `
        --startup-project $StartupProject `
        --output-dir Persistence/Migrations/AspNetIdentityDb `
        --verbose
    
    if ($LASTEXITCODE -ne 0) {
        throw "Migration creation failed"
    }
    
    Write-Host "- Migration created successfully" -ForegroundColor Green
}
catch {
    Write-Host ""
    Write-Host "Error: Failed to create migration" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

# ============================================
# Step 3: Generate SQL Script
# ============================================
Write-Host ""
Write-Host "[Step 3/4] Generating SQL script..." -ForegroundColor Yellow
Write-Host ""

$SqlScriptPath = Join-Path $IdentityMigrations "AspNetIdentityDb.sql"

try {
    dotnet ef migrations script `
        --context IdentityDbContext `
        --project $ProjectPath `
        --startup-project $StartupProject `
        --output $SqlScriptPath `
        --idempotent `
        --verbose
    
    if ($LASTEXITCODE -ne 0) {
        throw "SQL script generation failed"
    }
    
    Write-Host "- SQL script generated: AspNetIdentityDb.sql" -ForegroundColor Green
}
catch {
    Write-Host ""
    Write-Host "Error: Failed to generate SQL script" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

# ============================================
# Step 4: Update Database
# ============================================
Write-Host ""
Write-Host "[Step 4/4] Updating database..." -ForegroundColor Yellow
Write-Host ""

try {
    dotnet ef database update `
        --context IdentityDbContext `
        --project $ProjectPath `
        --startup-project $StartupProject `
        --verbose
    
    if ($LASTEXITCODE -ne 0) {
        throw "Database update failed"
    }
    
    Write-Host "- Database updated successfully" -ForegroundColor Green
}
catch {
    Write-Host ""
    Write-Host "Error: Failed to update database" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Check connection string in appsettings.json"
    Write-Host "2. Verify SQL Server is running"
    Write-Host "3. Ensure database user has proper permissions"
    Read-Host "Press Enter to exit"
    exit 1
}

# ============================================
# Success Summary
# ============================================
Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "✓ Migration Completed Successfully!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host ""
Write-Host "Generated Files:"
Write-Host "- Migration Classes: $IdentityMigrations\*.cs"
Write-Host "- SQL Script: $SqlScriptPath"
Write-Host ""
Write-Host "Database: Updated and ready to use" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host ""
Read-Host "Press Enter to exit"
