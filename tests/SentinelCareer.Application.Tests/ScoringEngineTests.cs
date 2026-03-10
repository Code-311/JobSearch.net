using SentinelCareer.Application.Options;
using SentinelCareer.Application.Services;
using SentinelCareer.Domain.Entities;

namespace SentinelCareer.Application.Tests;

public class ScoringEngineTests
{
    [Fact]
    public void Scores_India_Senior_Role_Highly()
    {
        var engine = new ScoringEngine(new ScoringOptions());
        var opp = new JobOpportunity { Title = "Director Operations", Description = "defence manufacturing", IsIndiaPriority = true };
        var score = engine.Score(opp, 6000000, 0.9m, 1, 80);
        Assert.True(score.CompositeScore >= 80);
        Assert.Contains("India-first", score.Explanation);
    }

    [Fact]
    public void Penalizes_Excluded_Roles()
    {
        var engine = new ScoringEngine(new ScoringOptions());
        var opp = new JobOpportunity { Title = "Senior Software Engineer", Description = "developer role", IsIndiaPriority = true };
        var score = engine.Score(opp, 6000000, 0.9m, 1, 80);
        Assert.True(score.CompositeScore < 80);
    }

    [Fact]
    public void MidLevel_GenericOps_ShouldNotOverscore()
    {
        var engine = new ScoringEngine(new ScoringOptions());
        var opp = new JobOpportunity { Title = "Operations Manager", Description = "consumer retail operations", IsIndiaPriority = true };
        var score = engine.Score(opp, 3800000, 0.8m, 2, 60);
        Assert.True(score.CompositeScore < 65);
    }

    [Fact]
    public void ThresholdTransition_ManualReviewBoundary()
    {
        var engine = new ScoringEngine(new ScoringOptions());
        var opp = new JobOpportunity { Title = "AVP Governance", Description = "manufacturing governance", IsIndiaPriority = true };
        var score = engine.Score(opp, 3500000, 0.8m, 5, 65);
        Assert.True(score.CompositeScore >= 65);
    }
}
