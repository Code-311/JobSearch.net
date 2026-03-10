# Architecture Summary
SentinelCareer uses a modular monorepo with .NET-first services and optional Python workers.
Data flow: Source registry -> Adapter ingestion -> Normalization -> Dedupe/merge -> Deterministic scoring -> alerts + dashboard + interview prep.
