param(
  [string]$Database = "sentinelcareer",
  [Parameter(Mandatory = $true)][string]$Input
)

psql -d $Database -f $Input
Write-Host "Restore complete from: $Input"
