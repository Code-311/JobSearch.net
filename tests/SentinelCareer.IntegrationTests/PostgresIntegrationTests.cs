using Microsoft.EntityFrameworkCore;
using Npgsql;
using SentinelCareer.Domain.Entities;
using SentinelCareer.Domain.Enums;
using SentinelCareer.Infrastructure.Data;
using SentinelCareer.Infrastructure.Repositories;

namespace SentinelCareer.IntegrationTests;

public class PostgresIntegrationTests
{
    private static string? ConnectionString => Environment.GetEnvironmentVariable("SENTINELCAREER_TEST_PG");

    [Fact]
    public async Task CanConnectAndSeeCoreTables()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString)) return;

        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand("select to_regclass('public.\"JobOpportunities\"') is not null", conn);
        var exists = (bool)(await cmd.ExecuteScalarAsync() ?? false);
        Assert.True(exists);
    }

    [Fact]
    public async Task SourceRunLifecycle_Persists()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString)) return;

        var options = new DbContextOptionsBuilder<SentinelCareerDbContext>().UseNpgsql(ConnectionString).Options;
        await using var db = new SentinelCareerDbContext(options);
        var sourceRepo = new SourceRepository(db);

        var run = await sourceRepo.StartRunAsync("company-careers", CancellationToken.None);
        await sourceRepo.CompleteRunAsync(run.Id, "Completed", "integration complete", CancellationToken.None);

        var persisted = await db.SourceRuns.FirstOrDefaultAsync(x => x.Id == run.Id);
        Assert.NotNull(persisted);
        Assert.Equal("Completed", persisted!.Status);
        Assert.NotNull(persisted.CompletedAt);
    }

    [Fact]
    public async Task ScoreHistory_And_InterviewPrep_Persist()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString)) return;

        var options = new DbContextOptionsBuilder<SentinelCareerDbContext>().UseNpgsql(ConnectionString).Options;
        await using var db = new SentinelCareerDbContext(options);

        var opp = new JobOpportunity
        {
            ExternalId = $"it-{Guid.NewGuid():N}",
            Title = "Director Governance",
            NormalizedTitle = "director governance",
            Description = "integration test",
            PublishedAt = DateTimeOffset.UtcNow,
            Company = new Company { Name = "IT Co", NormalizedName = "it co" },
            Location = new Location { City = "Mumbai", Region = "MH", Country = "India" },
            IsIndiaPriority = true
        };

        var oppRepo = new OpportunityRepository(db);
        var scoreRepo = new ScoreRepository(db);
        await oppRepo.UpsertAsync(opp, CancellationToken.None);

        var saved = await oppRepo.FindByExternalIdAsync(opp.ExternalId, CancellationToken.None);
        Assert.NotNull(saved);

        await scoreRepo.SaveScoreAsync(new OpportunityScore
        {
            OpportunityId = saved!.Id,
            CompositeScore = 83,
            ProfileFit = 90,
            CompensationFit = 70,
            GeographyFit = 95,
            SeniorityFit = 92,
            IndustryFit = 88,
            SourceCredibility = 80,
            Freshness = 90,
            OpportunityPotential = 75,
            Explanation = "integration",
            RecommendedNextAction = "Pursue immediately"
        }, CancellationToken.None);

        db.InterviewPrepPacks.Add(new InterviewPrepPack { OpportunityId = saved.Id, CompanyBrief = "brief", RoleBrief = "role", FitAnalysis = "fit" });
        await db.SaveChangesAsync();

        var hist = await scoreRepo.GetHistoryAsync(saved.Id, CancellationToken.None);
        Assert.NotEmpty(hist);
        var prep = await db.InterviewPrepPacks.FirstOrDefaultAsync(x => x.OpportunityId == saved.Id);
        Assert.NotNull(prep);
    }

    [Fact]
    public async Task ReviewWorkflow_Persists_Status_And_Notes()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString)) return;

        var options = new DbContextOptionsBuilder<SentinelCareerDbContext>().UseNpgsql(ConnectionString).Options;
        await using var db = new SentinelCareerDbContext(options);
        var repo = new OpportunityRepository(db);

        var opp = new JobOpportunity
        {
            ExternalId = $"it-{Guid.NewGuid():N}",
            Title = "Director Governance",
            NormalizedTitle = "director governance",
            Description = "integration test",
            PublishedAt = DateTimeOffset.UtcNow,
            Company = new Company { Name = "IT Co", NormalizedName = "it co" },
            Location = new Location { City = "Mumbai", Region = "MH", Country = "India" },
            IsIndiaPriority = true
        };

        await repo.UpsertAsync(opp, CancellationToken.None);
        var saved = await repo.FindByExternalIdAsync(opp.ExternalId, CancellationToken.None);
        Assert.NotNull(saved);

        await repo.UpdateReviewAsync(saved!.Id, OpportunityStatus.Ignored, "integration note", CancellationToken.None);
        var updated = await repo.GetAsync(saved.Id, CancellationToken.None);

        Assert.Equal(OpportunityStatus.Ignored, updated!.Status);
        Assert.Equal("integration note", updated.OperatorNotes);
    }
}
