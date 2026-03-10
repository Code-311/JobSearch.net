# Calibration report

## Issues found
- Mid-level roles with generic operations language could score too high.
- Compensation parsing had weak handling for ranges/crore/currency variants.
- Notification semantics needed explicit codification for new>=80 and +5 improvements.
- Triage flow needed operator decisioning and clearer score movement/history.
- Worker retries needed stronger timeout/jitter/per-item failure isolation.

## Calibration changes applied
- Updated deterministic scoring rules to penalize mid-level cues and out-of-sector generic operations context.
- Strengthened exclusion penalties for engineering/analyst/sales cues.
- Improved compensation parsing/conversion logic for LPA/lakh/crore and USD/EUR/GBP.
- Added review workflow persistence (`Status`, `OperatorNotes`) with dashboard controls.
- Added score delta and score history display in triage.
- Added source pack seeding for validation coverage counts.
- Added tests for compensation edges, duplicate boundary behavior, score threshold transitions, parser edge cases, and notification semantics.

## Known limitations
- Validation sources are configured as representative endpoints; production source-specific selectors still require site-by-site hardening.
- Windows desktop notifications are still a logger-backed stub.
- Full integration test against live PostgreSQL was not executable in this environment due missing `dotnet` runtime.

## Recommended next phase
- Run full Windows local validation against real endpoints and tune source-specific parsers.
- Add PostgreSQL-backed integration suite for ingestion/reconciliation workflows.
- Replace notification stub with real Windows toast integration.
- Expand migration history to fully align all domain entities and future review fields.
