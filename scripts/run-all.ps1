Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project src/SentinelCareer.Workers"
Start-Sleep -Seconds 2
dotnet run --project src/SentinelCareer.Web
