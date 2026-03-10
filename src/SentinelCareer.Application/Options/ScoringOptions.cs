using SentinelCareer.Contracts.Scoring;

namespace SentinelCareer.Application.Options;

public class ScoringOptions
{
    public const string Section = "Scoring";
    public int BaselineCompInrLpa { get; set; } = 35;
    public int AspirationalCompInrLpa { get; set; } = 50;
    public ScoringWeights Weights { get; set; } = new(0.2m, 0.15m, 0.15m, 0.15m, 0.1m, 0.1m, 0.05m, 0.1m);
}
