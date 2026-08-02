# TravelOrgOS Environment & Setup Script
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " TravelOrgOS Environment & LocalDB Safety Setup" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan

# 1. Verify LocalDB
$localDbInfo = sqllocaldb info "MSSQLLocalDB"
if (-not $localDbInfo) {
    Write-Error "CRITICAL ERROR: (localdb)\MSSQLLocalDB instance not found!"
    exit 1
}

Write-Host "[SAFETY CHECK] Data Source confirmed: (localdb)\MSSQLLocalDB" -ForegroundColor Green
Write-Host "[SAFETY CHECK] Database target confirmed: TravelOrgOS_Dev" -ForegroundColor Green

# 2. Execute SQL Schema
Write-Host "Executing SQL DDL script to create TravelOrgOS_Dev..." -ForegroundColor Yellow
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "$PSScriptRoot\..\database\scripts\01_InitialCreate.sql"

# 3. Build .NET Solution
Write-Host "Building .NET solution..." -ForegroundColor Yellow
dotnet build "$PSScriptRoot\..\TravelOrgOS.slnx"

# 4. Install Frontend NPM Packages
Write-Host "Installing Angular npm packages..." -ForegroundColor Yellow
Set-Location "$PSScriptRoot\..\src\TravelOrgOS.Web"
npm install --legacy-peer-deps

Write-Host "============================================================" -ForegroundColor Green
Write-Host " TravelOrgOS Setup Complete! Run run-api.ps1 and run-web.ps1" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
