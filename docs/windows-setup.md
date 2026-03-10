# Windows setup

## Prerequisites
1. Install PostgreSQL (`psql`, `pg_dump`, `createdb` available in PATH).
2. Install .NET SDK 8.x.
3. Ensure PowerShell (`pwsh`) available.

## One-command local setup
```powershell
pwsh ./scripts/setup-local.ps1
```

## One-command startup
```powershell
pwsh ./scripts/start-local.ps1
```

## Individual commands
- Migrations only: `pwsh ./scripts/apply-migrations.ps1`
- Workers only: `pwsh ./scripts/run-workers.ps1`
- Web only: `pwsh ./scripts/run-web.ps1`

## Graceful shutdown
- Stop web with `Ctrl+C` in the web console.
- Stop workers by closing worker console (or `Ctrl+C` if foreground).
- Hosted services are cancellation-aware and stop safely.

## Backup and restore
- Backup: `pwsh ./scripts/backup-db.ps1`
- Restore: `pwsh ./scripts/restore-db.ps1 -Input .\backup\sentinelcareer-YYYYMMDD-HHMMSS.sql`
