# Operator runbook

## Setup / startup
1. `pwsh ./scripts/setup-local.ps1`
2. `pwsh ./scripts/start-local.ps1`

## Windows host E2E validation
- Run `pwsh ./scripts/windows-e2e-validate.ps1`.
- Run `pwsh ./scripts/review-host-evidence.ps1`.
- Review files in `artifacts/host-validation`.

## Validate end-to-end local execution
- PostgreSQL running and reachable.
- Migration applied (`JobOpportunities` table exists).
- Seed loaded (validation pack sources/watchlist).
- Web UI reachable.
- Worker logs show scheduled ingestion/scoring cycles.
- Source run statuses persisted (`SourceRuns` table).
- Notification attempt logged/visible.
- Shutdown is graceful.

## Operations
- Monitor logs for `SourceRunStart/End`, retry warnings, and source-level run status.
- Use Opportunities page to mark pursue/review/ignore and capture notes.
- Verify score history and score delta movement for key opportunities.
- Confirm alerts fire only for new >=80 and +5 score improvements.

## Troubleshooting
- Source fetch failure: check `AdapterFetchFallback` entries and endpoint env vars (`SENTINELCAREER_COMPANY_SOURCE`, `SENTINELCAREER_EXEC_SOURCE`).
- Parser empty: check `AdapterParseEmpty` and inspect endpoint HTML drift.
- No notifications: verify Windows session notifications enabled and check worker logs for PowerShell toast errors.
- If a source repeatedly fails, disable it in `Sources` table and continue.
- If DB corruption suspected, restore from last backup using `restore-db.ps1`.
