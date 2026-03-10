# Windows host validation guide

Run end-to-end host validation and capture evidence:

```powershell
pwsh ./scripts/windows-e2e-validate.ps1
```

Then review evidence completeness + quick diagnostics:

```powershell
pwsh ./scripts/review-host-evidence.ps1
```

Evidence output directory:
- `artifacts/host-validation/validation.log`
- `artifacts/host-validation/summary.txt`
- `artifacts/host-validation/restore.log`
- `artifacts/host-validation/build.log`
- `artifacts/host-validation/migrate.log`
- `artifacts/host-validation/web.log`
- `artifacts/host-validation/worker.log`
- `artifacts/host-validation/api-opportunities.json`
- `artifacts/host-validation/source-run-summary.txt`
- `artifacts/host-validation/opportunity-count.txt`
- `artifacts/host-validation/score-history-count.txt`
- `artifacts/host-validation/notification-evidence.txt`
- `artifacts/host-validation/evidence-review.txt` (includes quick diagnostics)

Optional screenshot capture after web starts:
- Capture `/` and `/Opportunities/Index` manually using Windows Snipping Tool and store under `artifacts/host-validation/screenshots/`.

Notification evidence:
- Verify toast appears on Windows desktop.
- If no toast appears, inspect `worker.log` and `notification-evidence.txt`.
