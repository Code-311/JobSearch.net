using SentinelCareer.Domain.Enums;

namespace SentinelCareer.Domain.Rules;

public static class ScoreBandClassifier
{
    public static ScoreBand Classify(int score) => score switch
    {
        >= 80 => ScoreBand.PursueImmediately,
        >= 65 => ScoreBand.ManualReview,
        _ => ScoreBand.TrackOnly
    };
}
