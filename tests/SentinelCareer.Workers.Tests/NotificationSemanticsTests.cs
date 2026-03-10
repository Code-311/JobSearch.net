using SentinelCareer.Workers.Services;

namespace SentinelCareer.Workers.Tests;

public class NotificationSemanticsTests
{
    [Fact]
    public void NewHighPriority_ShouldNotify()
    {
        Assert.True(ScoringWorker.ShouldNotify(null, 80, out _));
    }

    [Fact]
    public void SmallIncrease_ShouldNotNotify()
    {
        Assert.False(ScoringWorker.ShouldNotify(78, 82, out _));
    }

    [Fact]
    public void LargeIncrease_ShouldNotify()
    {
        Assert.True(ScoringWorker.ShouldNotify(70, 76, out var reason));
        Assert.Contains("improved", reason);
    }
}
