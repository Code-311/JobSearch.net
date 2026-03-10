using SentinelCareer.Application.Services;

namespace SentinelCareer.Application.Tests;

public class NormalizationServiceTests
{
    [Fact]
    public void NormalizeCompensation_ParsesLpa()
    {
        var svc = new NormalizationService();
        var inr = svc.NormalizeCompensationToInr("INR 55 LPA", new Dictionary<string, decimal>());
        Assert.Equal(5500000, inr);
    }

    [Fact]
    public void NormalizeCompensation_ParsesUsdRange()
    {
        var svc = new NormalizationService();
        var inr = svc.NormalizeCompensationToInr("USD 120000 - 140000", new Dictionary<string, decimal> { ["USD"] = 83m });
        Assert.Equal(11620000, inr);
    }

    [Fact]
    public void NormalizeCompensation_ParsesCrore()
    {
        var svc = new NormalizationService();
        var inr = svc.NormalizeCompensationToInr("INR 1.2 crore", new Dictionary<string, decimal>());
        Assert.Equal(12000000, inr);
    }

    [Fact]
    public void IsExcludedRole_DetectsSoftwareEngineering()
    {
        var svc = new NormalizationService();
        Assert.True(svc.IsExcludedRole("Senior Software Engineer", "backend platform"));
    }
}
