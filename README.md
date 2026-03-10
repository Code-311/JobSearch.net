# SentinelCareer

SentinelCareer is a Windows-local AI-assisted job intelligence platform for a single operator, focused on India-first senior opportunities in operations/governance/risk/transformation leadership.

## Architecture
- `SentinelCareer.Web`: ASP.NET Core Razor dashboard.
- `SentinelCareer.Workers`: background ingestion/scoring workers (Windows service-capable).
- `SentinelCareer.Domain`: entities and deterministic scoring rules.
- `SentinelCareer.Application`: use-cases, normalization, scoring engine, dedupe, interview prep orchestration.
- `SentinelCareer.Infrastructure`: EF Core + Npgsql, adapters, repositories, notifications, source-run persistence, seeding.
- `SentinelCareer.PythonWorkers`: optional parsing/model helpers.

## Windows-local productionization highlights
- One-command setup: `pwsh ./scripts/setup-local.ps1`
- One-command startup: `pwsh ./scripts/start-local.ps1`
- One-command workers: `pwsh ./scripts/run-workers.ps1`
- Real Windows toast attempt via PowerShell + Windows runtime API with fallback logging.
- Source-run start/end persistence for troubleshooting.
- Backup/restore scripts for local PostgreSQL (`backup-db.ps1`, `restore-db.ps1`).

## Validation & calibration pack (current)
Seeded validation pack includes:
- 10 watchlist companies
- 5 executive search sources
- 3 public-sector/strategic portals
- 3 event/expo/industry sources
- 3 tender/EOI/procurement sources
- 2 selective job boards

## Windows local setup
1. Install .NET SDK 8+ and PostgreSQL 15+.
2. Run `pwsh ./scripts/setup-local.ps1`.
3. Start app stack with `pwsh ./scripts/start-local.ps1`.
4. Validate host E2E + evidence capture with `pwsh ./scripts/windows-e2e-validate.ps1`.
5. Verify evidence completeness with `pwsh ./scripts/review-host-evidence.ps1`.

## Backup and restore
- Backup: `pwsh ./scripts/backup-db.ps1`
- Restore: `pwsh ./scripts/restore-db.ps1 -Input .\backup\sentinelcareer-YYYYMMDD-HHMMSS.sql`

## Testing
- Domain scoring threshold tests.
- Application scoring/normalization/dedupe tests.
- Infrastructure adapter tests.
- Worker notification semantics tests.
- PostgreSQL integration tests (when `SENTINELCAREER_TEST_PG` is set).
- Python parser tests.

## Docs
- `docs/windows-setup.md`
- `docs/runbook.md`
- `docs/productionization-report.md`
- `docs/calibration-report.md`
- `docs/windows-host-validation.md`
- `docs/host-validation-report.md`
- `docs/remediation-report.md`
