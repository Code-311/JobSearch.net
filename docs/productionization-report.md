# Productionization report

## Issues found
- Notification implementation was a logging stub, not real Windows toast.
- Source-run lifecycle status was not persisted for troubleshooting.
- Source adapters had weak live-endpoint resilience and limited fallback parsing.
- Integration coverage for PostgreSQL persistence workflows was shallow.
- Windows-local operations lacked explicit one-command setup/start and backup/restore scripts.

## Fixes applied
- Added Windows-host E2E validation script (`scripts/windows-e2e-validate.ps1`) and host validation documentation/report template.
- Implemented real Windows-local toast attempt using PowerShell + Windows runtime Toast API, with safe fallback logging.
- Added source-run persistence methods and wired worker to start/complete source run records.
- Improved adapter anti-brittleness patterns with HTTP fetch fallback, selector fallback extraction, and source-specific logs.
- Added PostgreSQL-backed integration tests for migration/table visibility and review workflow persistence (env-driven).
- Added one-command scripts: setup/start and backup/restore.
- Updated Windows setup and runbook docs with troubleshooting and recovery guidance.

## Remaining limitations
- Full automated E2E execution could not be run in this environment because `dotnet` is unavailable; use the host validation script on Windows.
- Live source parsers are still generic templates; source-specific selectors need iterative tuning per endpoint.
- Toast behavior depends on Windows runtime availability and PowerShell execution policy.

## Recommended next phase
- Run full Windows laptop validation pass and capture runtime logs/screenshots.
- Add source-specific parser contract tests with HTML fixtures per high-priority endpoint.
- Add scheduled backup rotation and restore verification job.
- Expand integration suite with source-run and score-history round-trip assertions using dedicated test DB.
