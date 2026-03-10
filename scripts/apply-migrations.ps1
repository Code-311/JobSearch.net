param(
  [string]$Database = "sentinelcareer",
  [string]$SqlFile = "src/SentinelCareer.Infrastructure/Migrations/001_initial.sql"
)

Write-Host "Applying baseline schema to $Database using $SqlFile"
psql -d $Database -f $SqlFile
