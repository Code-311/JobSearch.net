Write-Host "Starting SentinelCareer workers and web..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project src/SentinelCareer.Workers"
Start-Sleep -Seconds 3
dotnet run --project src/SentinelCareer.Web
