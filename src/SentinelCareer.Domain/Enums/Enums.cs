namespace SentinelCareer.Domain.Enums;

public enum SourceType { CompanyCareers, ExecutiveSearch, JobBoard, GovernmentPortal, EventCalendar, NewsFeed, TenderPortal, ManualImport }
public enum CadenceTier { Tier1, Tier2, Daily, Nightly }
public enum CrawlMethod { Api, Scrape, Rss, Manual }
public enum WorkMode { Onsite, Hybrid, Remote, TravelHeavy }
public enum OpportunityStatus { New, Tracked, Shortlisted, Ignored, Archived }
public enum ScoreBand { TrackOnly, ManualReview, PursueImmediately }
