# Scoring model

Required sub-scores:
- profile_fit
- compensation_fit
- geography_fit
- seniority_fit
- industry_fit
- source_credibility
- freshness
- opportunity_potential

Bands:
- >=80 pursue immediately
- 65-79 review manually
- <65 track only

Hardening notes:
- India-first and seniority alignment are positively weighted.
- Compensation uses baseline 35L and premium 50L thresholds.
- Explicit penalties are applied for excluded role families (engineering-heavy, SOC analyst, sales-only cues).
- Explanation text now includes deterministic reason statements for operator triage.
