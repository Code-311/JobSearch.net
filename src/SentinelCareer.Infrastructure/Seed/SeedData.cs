using SentinelCareer.Domain.Entities;
using SentinelCareer.Domain.Enums;
using SentinelCareer.Infrastructure.Data;

namespace SentinelCareer.Infrastructure.Seed;

public static class SeedData
{
    public static async Task EnsureAsync(SentinelCareerDbContext db, CancellationToken cancellationToken)
    {
        if (!db.Sources.Any())
        {
            db.Sources.AddRange(BuildValidationSources());
        }

        if (!db.WatchlistCompanies.Any())
        {
            db.WatchlistCompanies.AddRange([
                new WatchlistCompany { Name = "HAL", Reason = "Defence aerospace strategic hiring" },
                new WatchlistCompany { Name = "Bharat Electronics", Reason = "Strategic electronics and defence programs" },
                new WatchlistCompany { Name = "BEML", Reason = "Heavy engineering and strategic manufacturing" },
                new WatchlistCompany { Name = "L&T", Reason = "Infrastructure and industrial leadership opportunities" },
                new WatchlistCompany { Name = "Tata Advanced Systems", Reason = "Aerospace and homeland security scale-up" },
                new WatchlistCompany { Name = "Boeing India", Reason = "Aerospace operations and governance" },
                new WatchlistCompany { Name = "Airbus India", Reason = "Aerospace capability expansion" },
                new WatchlistCompany { Name = "Siemens India", Reason = "Industrial digital and manufacturing transformations" },
                new WatchlistCompany { Name = "Honeywell India", Reason = "Strategic technology and operations leadership" },
                new WatchlistCompany { Name = "KPMG India", Reason = "Governance/risk/compliance consulting roles" }
            ]);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static IReadOnlyList<Source> BuildValidationSources()
    {
        return [
            // 5 executive search sources
            NewSource("Apex Executive Search", SourceType.ExecutiveSearch, CadenceTier.Tier2, CrawlMethod.Manual, "https://example.com/es/apex"),
            NewSource("Crest Leadership Search", SourceType.ExecutiveSearch, CadenceTier.Tier2, CrawlMethod.Scrape, "https://example.com/es/crest"),
            NewSource("Northbridge Board Search", SourceType.ExecutiveSearch, CadenceTier.Tier2, CrawlMethod.Scrape, "https://example.com/es/northbridge"),
            NewSource("StratEdge Partners", SourceType.ExecutiveSearch, CadenceTier.Tier2, CrawlMethod.Manual, "https://example.com/es/stratedge"),
            NewSource("Axis CXO Search", SourceType.ExecutiveSearch, CadenceTier.Tier2, CrawlMethod.Scrape, "https://example.com/es/axis"),

            // 3 public-sector / strategic portals
            NewSource("PSU Careers Portal", SourceType.GovernmentPortal, CadenceTier.Daily, CrawlMethod.Scrape, "https://example.com/psu/careers"),
            NewSource("Defence Procurement Public Notices", SourceType.GovernmentPortal, CadenceTier.Daily, CrawlMethod.Rss, "https://example.com/defence/notices"),
            NewSource("Strategic Institutions Careers", SourceType.GovernmentPortal, CadenceTier.Daily, CrawlMethod.Scrape, "https://example.com/strategic/careers"),

            // 3 event / expo / industry sources
            NewSource("Industrial Expo Calendar", SourceType.EventCalendar, CadenceTier.Daily, CrawlMethod.Rss, "https://example.com/events/industrial"),
            NewSource("Defence Summit Agenda", SourceType.EventCalendar, CadenceTier.Daily, CrawlMethod.Scrape, "https://example.com/events/defence"),
            NewSource("Governance Forum Events", SourceType.EventCalendar, CadenceTier.Daily, CrawlMethod.Rss, "https://example.com/events/governance"),

            // 3 tender/EOI/procurement
            NewSource("National Tender Wire", SourceType.TenderPortal, CadenceTier.Daily, CrawlMethod.Scrape, "https://example.com/tenders/national"),
            NewSource("Infrastructure EOI Tracker", SourceType.TenderPortal, CadenceTier.Daily, CrawlMethod.Scrape, "https://example.com/tenders/infra"),
            NewSource("Strategic Procurement Board", SourceType.TenderPortal, CadenceTier.Daily, CrawlMethod.Rss, "https://example.com/tenders/strategic"),

            // 2 selective job boards
            NewSource("Selective Leadership Board A", SourceType.JobBoard, CadenceTier.Tier1, CrawlMethod.Api, "https://example.com/jobs/lead-a"),
            NewSource("Selective Leadership Board B", SourceType.JobBoard, CadenceTier.Tier1, CrawlMethod.Api, "https://example.com/jobs/lead-b"),

            // company watchlist careers sources (10)
            NewSource("HAL Careers", SourceType.CompanyCareers, CadenceTier.Tier1, CrawlMethod.Scrape, "https://example.com/company/hal"),
            NewSource("BEL Careers", SourceType.CompanyCareers, CadenceTier.Tier1, CrawlMethod.Scrape, "https://example.com/company/bel"),
            NewSource("BEML Careers", SourceType.CompanyCareers, CadenceTier.Tier1, CrawlMethod.Scrape, "https://example.com/company/beml"),
            NewSource("L&T Careers", SourceType.CompanyCareers, CadenceTier.Tier1, CrawlMethod.Scrape, "https://example.com/company/lt"),
            NewSource("TASL Careers", SourceType.CompanyCareers, CadenceTier.Tier1, CrawlMethod.Scrape, "https://example.com/company/tasl"),
            NewSource("Boeing India Careers", SourceType.CompanyCareers, CadenceTier.Tier1, CrawlMethod.Scrape, "https://example.com/company/boeing"),
            NewSource("Airbus India Careers", SourceType.CompanyCareers, CadenceTier.Tier1, CrawlMethod.Scrape, "https://example.com/company/airbus"),
            NewSource("Siemens India Careers", SourceType.CompanyCareers, CadenceTier.Tier1, CrawlMethod.Scrape, "https://example.com/company/siemens"),
            NewSource("Honeywell India Careers", SourceType.CompanyCareers, CadenceTier.Tier1, CrawlMethod.Scrape, "https://example.com/company/honeywell"),
            NewSource("KPMG India Careers", SourceType.CompanyCareers, CadenceTier.Tier1, CrawlMethod.Scrape, "https://example.com/company/kpmg")
        ];
    }

    private static Source NewSource(string name, SourceType type, CadenceTier tier, CrawlMethod method, string endpoint)
        => new() { Name = name, SourceType = type, CadenceTier = tier, CrawlMethod = method, Endpoint = endpoint, LegalNotes = "Validation pack source" };
}
