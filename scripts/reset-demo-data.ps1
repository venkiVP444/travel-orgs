# Reset TravelOrgOS Demo Data
Write-Host "Resetting TravelOrgOS Demo Data via API..." -ForegroundColor Yellow
$response = Invoke-RestMethod -Uri "http://localhost:5100/api/demo/reset" -Method POST
Write-Host $response.message -ForegroundColor Green
