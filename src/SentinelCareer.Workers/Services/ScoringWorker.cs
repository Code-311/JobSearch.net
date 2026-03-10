using SentinelCareer.Application.Abstractions;
using SentinelCareer.Application.Services;
using SentinelCareer.Contracts.Notifications;
using SentinelCareer.Domain.Entities;

namespace SentinelCareer.Workers.Services;

public class ScoringWorker(IServiceProvider serviceProvider, ILogger<ScoringWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceProvider.CreateScope();
            var opportunities = await scope.ServiceProvider.GetRequiredService<IOpportunityRepository>().ListAsync(stoppingToken);
            var scoring = scope.ServiceProvider.GetRequiredService<ScoringEngine>();
            var scoreRepo = scope.ServiceProvider.GetRequiredService<IScoreRepository>();
            var notify = scope.ServiceProvider.GetRequiredService<INotificationDispatcher>();

            foreach (var opp in opportunities)
            {
                var result = scoring.Score(opp, 5500000, 0.9m, Math.Max(1, (DateTimeOffset.UtcNow - opp.PublishedAt).Days), 80);
                var previous = await scoreRepo.LatestForOpportunityAsync(opp.Id, stoppingToken);

                await scoreRepo.SaveScoreAsync(new OpportunityScore
                {
                    OpportunityId = opp.Id,
                    CompositeScore = result.CompositeScore,
                    ProfileFit = result.SubScores["profile_fit"],
                    CompensationFit = result.SubScores["compensation_fit"],
                    GeographyFit = result.SubScores["geography_fit"],
                    SeniorityFit = result.SubScores["seniority_fit"],
                    IndustryFit = result.SubScores["industry_fit"],
                    SourceCredibility = result.SubScores["source_credibility"],
                    Freshness = result.SubScores["freshness"],
                    OpportunityPotential = result.SubScores["opportunity_potential"],
                    Explanation = result.Explanation,
                    RecommendedNextAction = result.RecommendedNextAction
                }, stoppingToken);

                if (ShouldNotify(previous?.CompositeScore, result.CompositeScore, out var reason))
                {
                    await notify.NotifyDesktopAsync("SentinelCareer Alert", $"{opp.Title}: {result.CompositeScore} ({reason})", stoppingToken);
                }
            }

            logger.LogInformation("ScoringRunCompleted | Count={Count}", opportunities.Count);
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    public static bool ShouldNotify(int? previousScore, int currentScore, out string reason)
    {
        if (previousScore is null && currentScore >= 80)
        {
            reason = "new high-priority opportunity";
            return true;
        }

        if (previousScore is not null && currentScore - previousScore.Value >= 5)
        {
            reason = $"score improved by {currentScore - previousScore.Value}";
            return true;
        }

        reason = string.Empty;
        return false;
    }
}
