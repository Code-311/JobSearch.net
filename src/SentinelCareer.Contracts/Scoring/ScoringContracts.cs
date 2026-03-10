namespace SentinelCareer.Contracts.Scoring;

public record ScoringWeights(decimal ProfileFit, decimal CompensationFit, decimal GeographyFit, decimal SeniorityFit, decimal IndustryFit, decimal SourceCredibility, decimal Freshness, decimal OpportunityPotential);
public record ScoreReasons(IReadOnlyList<string> Reasons);
public record ScoredOpportunity(int CompositeScore, Dictionary<string, int> SubScores, string Explanation, string RecommendedNextAction);
