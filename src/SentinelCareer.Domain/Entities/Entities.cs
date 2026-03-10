using System.ComponentModel.DataAnnotations.Schema;
using SentinelCareer.Domain.Enums;

namespace SentinelCareer.Domain.Entities;

public class JobOpportunity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ExternalId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string NormalizedTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }
    public Guid? LocationId { get; set; }
    public Location? Location { get; set; }
    public WorkMode WorkMode { get; set; }
    public OpportunityStatus Status { get; set; } = OpportunityStatus.New;
    public string OperatorNotes { get; set; } = string.Empty;
    public DateTimeOffset PublishedAt { get; set; }
    public DateTimeOffset LastSeenAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsIndiaPriority { get; set; }
    [NotMapped] public OpportunityScore? LatestScore { get; set; }
    [NotMapped] public int ScoreDelta { get; set; }
}

public class Company
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string? Website { get; set; }
    public Guid? IndustryId { get; set; }
    public Industry? Industry { get; set; }
    public ICollection<JobOpportunity> Opportunities { get; set; } = new List<JobOpportunity>();
}

public class Industry { public Guid Id { get; set; } = Guid.NewGuid(); public string Name { get; set; } = string.Empty; public string Cluster { get; set; } = string.Empty; }
public class Location { public Guid Id { get; set; } = Guid.NewGuid(); public string City { get; set; } = string.Empty; public string Region { get; set; } = string.Empty; public string Country { get; set; } = "India"; }
public class CompensationSnapshot { public Guid Id { get; set; } = Guid.NewGuid(); public Guid JobOpportunityId { get; set; } public decimal? MinAmount { get; set; } public decimal? MaxAmount { get; set; } public string Currency { get; set; } = "INR"; public decimal? MinAmountInInr { get; set; } public decimal? MaxAmountInInr { get; set; } public DateTimeOffset CapturedAt { get; set; } = DateTimeOffset.UtcNow; }
public class Source { public Guid Id { get; set; } = Guid.NewGuid(); public string Name { get; set; } = string.Empty; public SourceType SourceType { get; set; } public CadenceTier CadenceTier { get; set; } public CrawlMethod CrawlMethod { get; set; } public string ParserVersion { get; set; } = "v1"; public string LegalNotes { get; set; } = string.Empty; public decimal ConfidenceWeight { get; set; } = 0.8m; public int DedupePriority { get; set; } = 100; public bool Enabled { get; set; } = true; public string Endpoint { get; set; } = string.Empty; }
public class SourceRun { public Guid Id { get; set; } = Guid.NewGuid(); public Guid SourceId { get; set; } public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.UtcNow; public DateTimeOffset? CompletedAt { get; set; } public string Status { get; set; } = "Running"; public string Message { get; set; } = string.Empty; }
public class OpportunitySourceLink { public Guid Id { get; set; } = Guid.NewGuid(); public Guid OpportunityId { get; set; } public Guid SourceId { get; set; } public string SourceUrl { get; set; } = string.Empty; public DateTimeOffset FirstSeenAt { get; set; } = DateTimeOffset.UtcNow; public DateTimeOffset LastSeenAt { get; set; } = DateTimeOffset.UtcNow; }
public class OpportunityScore { public Guid Id { get; set; } = Guid.NewGuid(); public Guid OpportunityId { get; set; } public int CompositeScore { get; set; } public int ProfileFit { get; set; } public int CompensationFit { get; set; } public int GeographyFit { get; set; } public int SeniorityFit { get; set; } public int IndustryFit { get; set; } public int SourceCredibility { get; set; } public int Freshness { get; set; } public int OpportunityPotential { get; set; } public string Explanation { get; set; } = string.Empty; public string RecommendedNextAction { get; set; } = string.Empty; public DateTimeOffset ScoredAt { get; set; } = DateTimeOffset.UtcNow; }
public class OpportunityScoreHistory { public Guid Id { get; set; } = Guid.NewGuid(); public Guid OpportunityId { get; set; } public int CompositeScore { get; set; } public DateTimeOffset RecordedAt { get; set; } = DateTimeOffset.UtcNow; }
public class OpportunitySignal { public Guid Id { get; set; } = Guid.NewGuid(); public Guid? OpportunityId { get; set; } public string SignalType { get; set; } = string.Empty; public string Summary { get; set; } = string.Empty; public int Strength { get; set; } }
public class EventSignal { public Guid Id { get; set; } = Guid.NewGuid(); public string EventName { get; set; } = string.Empty; public string Location { get; set; } = string.Empty; public DateTimeOffset Date { get; set; } }
public class TenderSignal { public Guid Id { get; set; } = Guid.NewGuid(); public string Authority { get; set; } = string.Empty; public string TenderTitle { get; set; } = string.Empty; public DateTimeOffset CloseDate { get; set; } }
public class NetworkingTarget { public Guid Id { get; set; } = Guid.NewGuid(); public Guid OpportunityId { get; set; } public string Name { get; set; } = string.Empty; public string Role { get; set; } = string.Empty; public string WhyRelevant { get; set; } = string.Empty; public string SourceNote { get; set; } = string.Empty; }
public class InterviewPrepPack { public Guid Id { get; set; } = Guid.NewGuid(); public Guid OpportunityId { get; set; } public string CompanyBrief { get; set; } = string.Empty; public string RoleBrief { get; set; } = string.Empty; public string FitAnalysis { get; set; } = string.Empty; public string QuestionsAndAnswers { get; set; } = string.Empty; public string Entry3090Plan { get; set; } = string.Empty; public string StakeholderMap { get; set; } = string.Empty; public string KeyRisks { get; set; } = string.Empty; public string NegotiationNotes { get; set; } = string.Empty; public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow; }
public class WatchlistCompany { public Guid Id { get; set; } = Guid.NewGuid(); public string Name { get; set; } = string.Empty; public string Reason { get; set; } = string.Empty; public string PriorityTier { get; set; } = "A"; }
public class JobFamily { public Guid Id { get; set; } = Guid.NewGuid(); public string Name { get; set; } = string.Empty; }
public class RoleNormalizationMap { public Guid Id { get; set; } = Guid.NewGuid(); public string RawTitle { get; set; } = string.Empty; public string NormalizedTitle { get; set; } = string.Empty; public string SeniorityBand { get; set; } = string.Empty; }
public class CurrencyRateSnapshot { public Guid Id { get; set; } = Guid.NewGuid(); public string CurrencyCode { get; set; } = "USD"; public decimal InrRate { get; set; } public DateTimeOffset CapturedAt { get; set; } = DateTimeOffset.UtcNow; }
public class CrawlPolicy { public Guid Id { get; set; } = Guid.NewGuid(); public Guid SourceId { get; set; } public bool AllowOverlap { get; set; } public int RetryCount { get; set; } = 2; public int BackoffSeconds { get; set; } = 20; }
public class JobRunNotification { public Guid Id { get; set; } = Guid.NewGuid(); public Guid OpportunityId { get; set; } public string NotificationType { get; set; } = string.Empty; public string Message { get; set; } = string.Empty; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; }
