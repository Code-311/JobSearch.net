using Microsoft.Extensions.Logging.Abstractions;
using SentinelCareer.Infrastructure.Adapters;

namespace SentinelCareer.Infrastructure.Tests;

public class GenericAdapterTests
{
    [Fact]
    public async Task CompanyAdapter_ReturnsPayload()
    {
        var adapter = new GenericCompanyCareersAdapter(NullLogger<GenericCompanyCareersAdapter>.Instance, new TestHttpClientFactory());
        var items = await adapter.FetchAsync(CancellationToken.None);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task CompanyAdapter_RespectsCancellation()
    {
        var adapter = new GenericCompanyCareersAdapter(NullLogger<GenericCompanyCareersAdapter>.Instance, new TestHttpClientFactory());
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        await Assert.ThrowsAsync<OperationCanceledException>(() => adapter.FetchAsync(cts.Token));
    }

    [Fact]
    public void Parser_HandlesFixture_WithFallbacks()
    {
        var html = File.ReadAllText("Fixtures/company_jobs_fixture.html");
        var parsed = SourceHtmlParser.ParseSimpleCards(html, "Fixture Co", "https://fixture", _ => { });
        Assert.Equal(2, parsed.Count);
        Assert.Contains(parsed, x => x.Title.Contains("Senior Director", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(parsed, x => x.Compensation?.Contains("75", StringComparison.OrdinalIgnoreCase) == true);
    }

    [Fact]
    public void Parser_HandlesLabelFallbacks()
    {
        var html = File.ReadAllText("Fixtures/executive_search_fixture.html");
        var parsed = SourceHtmlParser.ParseSimpleCards(html, "Exec Co", "https://fixture", _ => { });
        Assert.Equal(2, parsed.Count);
        Assert.Contains(parsed, x => x.Location.Contains("Delhi", StringComparison.OrdinalIgnoreCase));
    }

    private sealed class TestHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new();
    }
}
