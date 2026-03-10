param(
  [string]$Database = "sentinelcareer"
)

Write-Host "[1/3] Ensuring database exists: $Database"
createdb $Database 2>$null

Write-Host "[2/3] Applying baseline migration"
pwsh ./scripts/apply-migrations.ps1 -Database $Database

Write-Host "[3/3] Build verification"
dotnet build SentinelCareer.sln
