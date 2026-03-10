using SentinelCareer.Domain.Entities;
using SentinelCareer.Domain.Enums;

namespace SentinelCareer.Application.Abstractions;

public interface IOpportunityRepository
{
    Task<IReadOnlyList<JobOpportunity>> ListAsync(CancellationToken cancellationToken);
    Task<JobOpportunity?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<JobOpportunity?> FindByExternalIdAsync(string externalId, CancellationToken cancellationToken);
    Task<JobOpportunity?> FindPotentialDuplicateAsync(string normalizedTitle, string companyName, string city, DateTimeOffset publishedAt, CancellationToken cancellationToken);
    Task UpsertAsync(JobOpportunity opportunity, CancellationToken cancellationToken);
    Task UpdateReviewAsync(Guid opportunityId, OpportunityStatus status, string notes, CancellationToken cancellationToken);
}

public interface ISourceRepository
{
    Task<IReadOnlyList<Source>> EnabledAsync(CancellationToken cancellationToken);
    Task<SourceRun> StartRunAsync(string adapterFamily, CancellationToken cancellationToken);
    Task CompleteRunAsync(Guid runId, string status, string message, CancellationToken cancellationToken);
}

public interface IScoreRepository
{
    Task SaveScoreAsync(OpportunityScore score, CancellationToken cancellationToken);
    Task<OpportunityScore?> LatestForOpportunityAsync(Guid opportunityId, CancellationToken cancellationToken);
    Task<IReadOnlyList<OpportunityScoreHistory>> GetHistoryAsync(Guid opportunityId, CancellationToken cancellationToken);
}
