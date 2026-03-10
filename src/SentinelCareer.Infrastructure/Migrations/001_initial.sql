-- Baseline schema migration for SentinelCareer (apply with psql -f)
create table if not exists "Companies" (
  "Id" uuid primary key,
  "Name" text not null,
  "NormalizedName" text not null,
  "Website" text null
);

create table if not exists "Locations" (
  "Id" uuid primary key,
  "City" text not null,
  "Region" text not null,
  "Country" text not null
);

create table if not exists "JobOpportunities" (
  "Id" uuid primary key,
  "ExternalId" text not null,
  "Title" text not null,
  "NormalizedTitle" text not null,
  "Description" text not null,
  "CompanyId" uuid references "Companies"("Id"),
  "LocationId" uuid references "Locations"("Id"),
  "Status" integer not null default 0,
  "OperatorNotes" text not null default '',
  "PublishedAt" timestamptz not null,
  "LastSeenAt" timestamptz not null,
  "IsIndiaPriority" boolean not null default false
);

create index if not exists "IX_JobOpportunities_ExternalId" on "JobOpportunities"("ExternalId");
create index if not exists "IX_JobOpportunities_Status" on "JobOpportunities"("Status");
