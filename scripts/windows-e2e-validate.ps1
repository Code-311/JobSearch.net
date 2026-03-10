param(
  [string]$Database = "sentinelcareer",
  [string]$EvidenceDir = "artifacts/host-validation"
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

New-Item -ItemType Directory -Force -Path $EvidenceDir | Out-Null

$log = Join-Path $EvidenceDir "validation.log"
$summary = Join-Path $EvidenceDir "summary.txt"
if (Test-Path $log) { Remove-Item $log -Force }
if (Test-Path $summary) { Remove-Item $summary -Force }

function Log([string]$msg) { $msg | Tee-Object -FilePath $log -Append }
function Run-Step([string]$name, [scriptblock]$action) {
  Log "[STEP] $name"
  try {
    & $action
    Log "[OK] $name"
  }
  catch {
    Log "[FAIL] $name :: $($_.Exception.Message)"
    throw
  }
}

$worker = $null
$web = $null

try {
  Run-Step "dotnet restore" { dotnet restore SentinelCareer.sln 2>&1 | Tee-Object -FilePath (Join-Path $EvidenceDir "restore.log") -Append }
  Run-Step "dotnet build" { dotnet build SentinelCareer.sln -c Release 2>&1 | Tee-Object -FilePath (Join-Path $EvidenceDir "build.log") -Append }
  Run-Step "ensure db" { createdb $Database 2>$null }
  Run-Step "migrations" { pwsh ./scripts/apply-migrations.ps1 -Database $Database 2>&1 | Tee-Object -FilePath (Join-Path $EvidenceDir "migrate.log") -Append }

  Run-Step "start workers" {
    $script:worker = Start-Process powershell -PassThru -WindowStyle Hidden -ArgumentList "-NoProfile", "-Command", "dotnet run --project src/SentinelCareer.Workers *> '$EvidenceDir\\worker.log'"
    Start-Sleep -Seconds 6
  }

  Run-Step "start web" {
    $script:web = Start-Process powershell -PassThru -WindowStyle Hidden -ArgumentList "-NoProfile", "-Command", "dotnet run --project src/SentinelCareer.Web *> '$EvidenceDir\\web.log'"
    Start-Sleep -Seconds 10
  }

  Run-Step "call API" {
    Invoke-WebRequest -Uri "http://localhost:5000/api/opportunities" -UseBasicParsing -TimeoutSec 25 |
      Select-Object -ExpandProperty Content |
      Out-File (Join-Path $EvidenceDir "api-opportunities.json")
  }

  Run-Step "capture DB evidence" {
    psql -d $Database -c "select \"Status\", count(*) from \"SourceRuns\" group by \"Status\";" | Out-File (Join-Path $EvidenceDir "source-run-summary.txt")
    psql -d $Database -c "select count(*) as opportunities from \"JobOpportunities\";" | Out-File (Join-Path $EvidenceDir "opportunity-count.txt")
    psql -d $Database -c "select count(*) as score_history from \"OpportunityScoreHistories\";" | Out-File (Join-Path $EvidenceDir "score-history-count.txt")
  }

  Run-Step "capture notification evidence" {
    if (Test-Path (Join-Path $EvidenceDir "worker.log")) {
      Select-String -Path (Join-Path $EvidenceDir "worker.log") -Pattern "DesktopNotification|SentinelCareer Alert" |
        Out-File (Join-Path $EvidenceDir "notification-evidence.txt")
    }
    if (!(Test-Path (Join-Path $EvidenceDir "notification-evidence.txt"))) {
      "No notification lines found in worker log." | Out-File (Join-Path $EvidenceDir "notification-evidence.txt")
    }
  }

  "ValidationStatus=Success`nTimestamp=$(Get-Date -Format s)" | Out-File $summary
}
catch {
  "ValidationStatus=Failure`nTimestamp=$(Get-Date -Format s)`nMessage=$($_.Exception.Message)" | Out-File $summary
  throw
}
finally {
  Log "[STEP] graceful shutdown"
  if ($web -and -not $web.HasExited) { Stop-Process -Id $web.Id }
  if ($worker -and -not $worker.HasExited) { Stop-Process -Id $worker.Id }
  Log "Validation complete. Evidence stored in $EvidenceDir"
}
