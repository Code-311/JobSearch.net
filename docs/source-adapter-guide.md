# Source adapter developer guide

Implement `ISourceAdapter` and register in Infrastructure DI.

Guidance:
- Keep parser versioned and source metadata maintained.
- Prefer resilient extraction patterns with fallback selectors.
- Use endpoint fetch + parse + fallback seed behavior to avoid total ingestion loss.
- Emit source-specific logs for fetch, parse success/fallback, and failure causes.
- Ensure cancellation support and bounded retries at worker layer.
- Normalize payload fields before persistence.
- Validate parser edge cases: missing location, duplicate cards, malformed compensation fields, and stale listings.

- Add endpoint-specific HTML fixtures under tests and assert parser behavior to reduce selector brittleness.
