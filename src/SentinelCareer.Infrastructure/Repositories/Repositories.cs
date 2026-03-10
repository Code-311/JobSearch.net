using Microsoft.EntityFrameworkCore;
using SentinelCareer.Application.Abstractions;
using SentinelCareer.Domain.Entities;
using SentinelCareer.Domain.Enums;
using SentinelCareer.Infrastructure.Data;

namespace SentinelCareer.Infrastructure.Repositories;

public class OpportunityRepository(SentinelCareerDbContext db) : IOpportunityRepository
{
    public async Task<JobOpportunity?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        await db.JobOpportunities.Include(x => x.Company).Include(x => x.Location).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<JobOpportunity>> ListAsync(CancellationToken cancellationToken)
    {
        var items = await db.JobOpportunities.Include(x => x.Company).Include(x => x.Location).OrderByDescending(x => x.PublishedAt).ToListAsync(cancellationToken);
        var ids = items.Select(x => x.Id).ToList();

        var scores = await db.OpportunityScores
            .Where(x => ids.Contains(x.OpportunityId))
            .OrderByDescending(x => x.ScoredAt)
            .ToListAsync(cancellationToken);

        var grouped = scores.GroupBy(x => x.OpportunityId).ToDictionary(g => g.Key, g => g.Take(2).ToList());
        foreach (var item in items)
        {
            var latestTwo = grouped.GetValueOrDefault(item.Id);
            item.LatestScore = latestTwo?.FirstOrDefault();
            item.ScoreDelta = latestTwo is { Count: >= 2 } ? latestTwo[0].CompositeScore - latestTwo[1].CompositeScore : 0;
        }

        return items;
    }

    public async Task<JobOpportunity?> FindByExternalIdAsync(string externalId, CancellationToken cancellationToken)
        => await db.JobOpportunities.FirstOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);

    public async Task<JobOpportunity?> FindPotentialDuplicateAsync(string normalizedTitle, string companyName, string city, DateTimeOffset publishedAt, CancellationToken cancellationToken)
    {
        var minDate = publishedAt.AddDays(-30);
        var maxDate = publishedAt.AddDays(30);
        return await db.JobOpportunities
            .Include(x => x.Company)
            .Include(x => x.Location)
            .Where(x => x.NormalizedTitle == normalizedTitle && x.PublishedAt >= minDate && x.PublishedAt <= maxDate)
            .FirstOrDefaultAsync(x => x.Company != null && x.Location != null &&
                                     x.Company.NormalizedName == companyName && x.Location.City.ToLower() == city,
                cancellationToken);
    }

    public async Task UpsertAsync(JobOpportunity opportunity, CancellationToken cancellationToken)
    {
        if (opportunity.Company is not null)
        {
            var existingCompany = await db.Companies.FirstOrDefaultAsync(x => x.NormalizedName == opportunity.Company.NormalizedName, cancellationToken);
            if (existingCompany is null)
            {
                db.Companies.Add(opportunity.Company);
                opportunity.CompanyId = opportunity.Company.Id;
            }
            else
            {
                opportunity.CompanyId = existingCompany.Id;
                opportunity.Company = existingCompany;
            }
        }

        if (opportunity.Location is not null)
        {
            var city = opportunity.Location.City.ToLowerInvariant();
            var country = opportunity.Location.Country.ToLowerInvariant();
            var existingLocation = await db.Locations.FirstOrDefaultAsync(x => x.City.ToLower() == city && x.Country.ToLower() == country, cancellationToken);
            if (existingLocation is null)
            {
                db.Locations.Add(opportunity.Location);
                opportunity.LocationId = opportunity.Location.Id;
            }
            else
            {
                opportunity.LocationId = existingLocation.Id;
                opportunity.Location = existingLocation;
            }
        }

        var existing = await db.JobOpportunities.FirstOrDefaultAsync(x => x.ExternalId == opportunity.ExternalId, cancellationToken);
        if (existing is null)
        {
            db.JobOpportunities.Add(opportunity);
        }
        else
        {
            existing.Title = opportunity.Title;
            existing.NormalizedTitle = opportunity.NormalizedTitle;
            existing.Description = opportunity.Description;
            existing.LastSeenAt = DateTimeOffset.UtcNow;
            existing.PublishedAt = opportunity.PublishedAt;
            existing.IsIndiaPriority = opportunity.IsIndiaPriority;
            existing.CompanyId = opportunity.CompanyId;
            existing.LocationId = opportunity.LocationId;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateReviewAsync(Guid opportunityId, OpportunityStatus status, string notes, CancellationToken cancellationToken)
    {
        var opportunity = await db.JobOpportunities.FirstOrDefaultAsync(x => x.Id == opportunityId, cancellationToken);
        if (opportunity is null) return;
        opportunity.Status = status;
        opportunity.OperatorNotes = notes.Trim();
        await db.SaveChangesAsync(cancellationToken);
    }
}

public class SourceRepository(SentinelCareerDbContext db) : ISourceRepository
{
    public async Task<IReadOnlyList<Source>> EnabledAsync(CancellationToken cancellationToken) =>
        await db.Sources.Where(x => x.Enabled).ToListAsync(cancellationToken);

    public async Task<SourceRun> StartRunAsync(string adapterFamily, CancellationToken cancellationToken)
    {
        var source = await ResolveSourceAsync(adapterFamily, cancellationToken);
        var run = new SourceRun
        {
            SourceId = source?.Id ?? Guid.Empty,
            StartedAt = DateTimeOffset.UtcNow,
            Status = "Running",
            Message = $"Start {adapterFamily}"
        };
        db.SourceRuns.Add(run);
        await db.SaveChangesAsync(cancellationToken);
        return run;
    }

    public async Task CompleteRunAsync(Guid runId, string status, string message, CancellationToken cancellationToken)
    {
        var run = await db.SourceRuns.FirstOrDefaultAsync(x => x.Id == runId, cancellationToken);
        if (run is null) return;
        run.CompletedAt = DateTimeOffset.UtcNow;
        run.Status = status;
        run.Message = message;
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<Source?> ResolveSourceAsync(string family, CancellationToken cancellationToken)
    {
        family = family.ToLowerInvariant();
        if (family.Contains("executive"))
            return await db.Sources.FirstOrDefaultAsync(x => x.SourceType == SourceType.ExecutiveSearch, cancellationToken);
        if (family.Contains("company"))
            return await db.Sources.FirstOrDefaultAsync(x => x.SourceType == SourceType.CompanyCareers, cancellationToken);
        if (family.Contains("signal") || family.Contains("event"))
            return await db.Sources.FirstOrDefaultAsync(x => x.SourceType == SourceType.EventCalendar || x.SourceType == SourceType.NewsFeed, cancellationToken);

        return await db.Sources.FirstOrDefaultAsync(cancellationToken);
    }
}

public class ScoreRepository(SentinelCareerDbContext db) : IScoreRepository
{
    public async Task<OpportunityScore?> LatestForOpportunityAsync(Guid opportunityId, CancellationToken cancellationToken) =>
        await db.OpportunityScores.Where(x => x.OpportunityId == opportunityId).OrderByDescending(x => x.ScoredAt).FirstOrDefaultAsync(cancellationToken);

    public async Task SaveScoreAsync(OpportunityScore score, CancellationToken cancellationToken)
    {
        db.OpportunityScores.Add(score);
        db.OpportunityScoreHistories.Add(new OpportunityScoreHistory { OpportunityId = score.OpportunityId, CompositeScore = score.CompositeScore });
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OpportunityScoreHistory>> GetHistoryAsync(Guid opportunityId, CancellationToken cancellationToken)
        => await db.OpportunityScoreHistories.Where(x => x.OpportunityId == opportunityId).OrderByDescending(x => x.RecordedAt).Take(10).ToListAsync(cancellationToken);
}
