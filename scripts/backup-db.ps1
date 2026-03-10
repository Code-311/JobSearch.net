param(
  [string]$Database = "sentinelcareer",
  [string]$Output = "backup/sentinelcareer-$(Get-Date -Format yyyyMMdd-HHmmss).sql"
)

New-Item -ItemType Directory -Force -Path (Split-Path $Output) | Out-Null
pg_dump -d $Database -f $Output
Write-Host "Backup complete: $Output"
