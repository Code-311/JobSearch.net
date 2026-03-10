using SentinelCareer.Domain.Enums;
using SentinelCareer.Domain.Rules;

namespace SentinelCareer.Domain.Tests;

public class ScoreBandClassifierTests
{
    [Theory]
    [InlineData(80, ScoreBand.PursueImmediately)]
    [InlineData(72, ScoreBand.ManualReview)]
    [InlineData(40, ScoreBand.TrackOnly)]
    public void Classify_Works(int score, ScoreBand expected) => Assert.Equal(expected, ScoreBandClassifier.Classify(score));
}
