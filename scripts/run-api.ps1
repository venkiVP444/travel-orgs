# Run TravelOrgOS ASP.NET Core Web API
Write-Host "Starting TravelOrgOS API Server on http://localhost:5100..." -ForegroundColor Cyan
Set-Location "$PSScriptRoot\..\src\TravelOrgOS.Api"
dotnet run --urls "http://localhost:5100"
