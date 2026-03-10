using SentinelCareer.Application.Services;

namespace SentinelCareer.Application.Tests;

public class DedupeServiceTests
{
    [Fact]
    public void BuildKey_IsStable()
    {
        var svc = new DedupeService();
        var k1 = svc.BuildKey("director operations", "acme ltd", "Mumbai");
        var k2 = svc.BuildKey("director operations", "acme ltd", "mumbai");
        Assert.Equal(k1, k2);
    }

    [Fact]
    public void DuplicateWindow_IsWithin30Days()
    {
        var svc = new DedupeService();
        Assert.True(svc.IsLikelyDuplicate(DateTimeOffset.UtcNow.AddDays(-10), DateTimeOffset.UtcNow));
        Assert.False(svc.IsLikelyDuplicate(DateTimeOffset.UtcNow.AddDays(-45), DateTimeOffset.UtcNow));
    }

    [Fact]
    public void DuplicateWindow_Boundary30Days_Included()
    {
        var svc = new DedupeService();
        Assert.True(svc.IsLikelyDuplicate(DateTimeOffset.UtcNow.AddDays(-30), DateTimeOffset.UtcNow));
    }
}
