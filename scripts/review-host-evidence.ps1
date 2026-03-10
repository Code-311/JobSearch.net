param(
  [string]$EvidenceDir = "artifacts/host-validation"
)

$ErrorActionPreference = "Stop"

if (!(Test-Path $EvidenceDir)) {
  Write-Host "Evidence directory not found: $EvidenceDir"
  exit 2
}

$required = @(
  "validation.log",
  "summary.txt",
  "restore.log",
  "build.log",
  "migrate.log",
  "web.log",
  "worker.log",
  "api-opportunities.json",
  "source-run-summary.txt",
  "opportunity-count.txt",
  "notification-evidence.txt"
)

$missing = @()
foreach ($f in $required) {
  if (!(Test-Path (Join-Path $EvidenceDir $f))) { $missing += $f }
}

$report = Join-Path $EvidenceDir "evidence-review.txt"
"Evidence review at $(Get-Date -Format s)" | Out-File $report
if ($missing.Count -gt 0) {
  "Missing files:" | Out-File $report -Append
  $missing | ForEach-Object { " - $_" | Out-File $report -Append }
}
else {
  "All required evidence files present." | Out-File $report -Append
}

if (Test-Path (Join-Path $EvidenceDir "summary.txt")) {
  "`nSummary:" | Out-File $report -Append
  Get-Content (Join-Path $EvidenceDir "summary.txt") | Out-File $report -Append
}

Write-Host "Evidence review written: $report"
if ($missing.Count -gt 0) { exit 1 }


# diagnostic extraction (best-effort)
$diag = @()
$webLog = Join-Path $EvidenceDir "web.log"
$workerLog = Join-Path $EvidenceDir "worker.log"
$migrateLog = Join-Path $EvidenceDir "migrate.log"

if (Test-Path $webLog) {
  if (Select-String -Path $webLog -Pattern "Address already in use|Failed to bind" -Quiet) { $diag += "Web port binding conflict detected." }
  if (Select-String -Path $webLog -Pattern "Npgsql|connection|password authentication failed" -Quiet) { $diag += "Web DB connection/authentication issue detected." }
}
if (Test-Path $workerLog) {
  if (Select-String -Path $workerLog -Pattern "SourceRunFailed" -Quiet) { $diag += "One or more source runs failed." }
  if (Select-String -Path $workerLog -Pattern "DesktopNotification powershell failed|DesktopNotification exception" -Quiet) { $diag += "Notification dispatch failure detected." }
}
if (Test-Path $migrateLog) {
  if (Select-String -Path $migrateLog -Pattern "ERROR:|error:" -Quiet) { $diag += "Migration errors detected." }
}

"`nDiagnostics:" | Out-File $report -Append
if ($diag.Count -eq 0) {
  " - No known error signatures detected by quick scan." | Out-File $report -Append
} else {
  $diag | Sort-Object -Unique | ForEach-Object { " - $_" | Out-File $report -Append }
}
