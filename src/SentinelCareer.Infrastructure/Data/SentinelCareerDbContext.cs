using Microsoft.EntityFrameworkCore;
using SentinelCareer.Domain.Entities;

namespace SentinelCareer.Infrastructure.Data;

public class SentinelCareerDbContext(DbContextOptions<SentinelCareerDbContext> options) : DbContext(options)
{
    public DbSet<JobOpportunity> JobOpportunities => Set<JobOpportunity>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Industry> Industries => Set<Industry>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<CompensationSnapshot> CompensationSnapshots => Set<CompensationSnapshot>();
    public DbSet<Source> Sources => Set<Source>();
    public DbSet<SourceRun> SourceRuns => Set<SourceRun>();
    public DbSet<OpportunityScore> OpportunityScores => Set<OpportunityScore>();
    public DbSet<OpportunityScoreHistory> OpportunityScoreHistories => Set<OpportunityScoreHistory>();
    public DbSet<InterviewPrepPack> InterviewPrepPacks => Set<InterviewPrepPack>();
    public DbSet<NetworkingTarget> NetworkingTargets => Set<NetworkingTarget>();
    public DbSet<WatchlistCompany> WatchlistCompanies => Set<WatchlistCompany>();
    public DbSet<JobRunNotification> JobRunNotifications => Set<JobRunNotification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobOpportunity>().HasIndex(x => x.ExternalId);
        modelBuilder.Entity<JobOpportunity>().HasIndex(x => x.Status);
        modelBuilder.Entity<Company>().HasIndex(x => x.NormalizedName);
        modelBuilder.Entity<Source>().HasIndex(x => x.Name).IsUnique();
    }
}
