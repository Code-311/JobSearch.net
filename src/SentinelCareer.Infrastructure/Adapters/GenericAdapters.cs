using SentinelCareer.Contracts.Ingestion;

namespace SentinelCareer.Infrastructure.Adapters;

public abstract class AdapterBase(ILogger logger, IHttpClientFactory httpClientFactory)
{
    protected ILogger Logger { get; } = logger;
    protected IHttpClientFactory HttpClientFactory { get; } = httpClientFactory;

    protected static IngestedOpportunity Build(string externalId, string title, string description, string company, string location, string country, string? compensation, string url, DateTimeOffset postedAt)
        => new(externalId.Trim(), title.Trim(), description.Trim(), company.Trim(), location.Trim(), country.Trim(), compensation, url.Trim(), postedAt);

    protected async Task<string?> TryFetchHtmlAsync(string endpoint, CancellationToken cancellationToken)
    {
        try
        {
            var client = HttpClientFactory.CreateClient("sources");
            client.Timeout = TimeSpan.FromSeconds(15);
            return await client.GetStringAsync(endpoint, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "AdapterFetchFallback | Endpoint={Endpoint}", endpoint);
            return null;
        }
    }
}

public class GenericCompanyCareersAdapter(ILogger<GenericCompanyCareersAdapter> logger, IHttpClientFactory httpClientFactory) : AdapterBase(logger, httpClientFactory), ISourceAdapter
{
    public string Family => "company-careers";

    public async Task<IReadOnlyList<IngestedOpportunity>> FetchAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var endpoint = Environment.GetEnvironmentVariable("SENTINELCAREER_COMPANY_SOURCE") ?? "https://example.com/jobs/1";
        var html = await TryFetchHtmlAsync(endpoint, cancellationToken);
        if (!string.IsNullOrWhiteSpace(html))
        {
            var parsed = SourceHtmlParser.ParseSimpleCards(html, "Strategic Manufacturing Ltd", endpoint, m => logger.LogInformation("{Message}", m));
            if (parsed.Count > 0)
            {
                logger.LogInformation("AdapterParsed | Family={Family} | Count={Count}", Family, parsed.Count);
                return parsed;
            }
            logger.LogWarning("AdapterParseEmpty | Family={Family} | Endpoint={Endpoint}", Family, endpoint);
        }

        logger.LogInformation("AdapterUsingFallbackSeed | Family={Family}", Family);
        return [
            Build("cmp-001", "Director, Operations Governance", "Lead transformation and governance across India plants", "Strategic Manufacturing Ltd", "Pune", "India", "INR 55 LPA", endpoint, DateTimeOffset.UtcNow.AddHours(-4))
        ];
    }
}

public class GenericExecutiveSearchAdapter(ILogger<GenericExecutiveSearchAdapter> logger, IHttpClientFactory httpClientFactory) : AdapterBase(logger, httpClientFactory), ISourceAdapter
{
    public string Family => "executive-search";

    public async Task<IReadOnlyList<IngestedOpportunity>> FetchAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var endpoint = Environment.GetEnvironmentVariable("SENTINELCAREER_EXEC_SOURCE") ?? "https://example.com/es/1";
        var html = await TryFetchHtmlAsync(endpoint, cancellationToken);
        if (!string.IsNullOrWhiteSpace(html))
        {
            var parsed = SourceHtmlParser.ParseSimpleCards(html, "Apex Search Partners", endpoint, m => logger.LogInformation("{Message}", m));
            if (parsed.Count > 0)
            {
                logger.LogInformation("AdapterParsed | Family={Family} | Count={Count}", Family, parsed.Count);
                return parsed;
            }
            logger.LogWarning("AdapterParseEmpty | Family={Family} | Endpoint={Endpoint}", Family, endpoint);
        }

        logger.LogInformation("AdapterUsingFallbackSeed | Family={Family}", Family);
        return [
            Build("es-001", "VP - Risk & Compliance", "Executive mandate for enterprise risk", "Apex Search Partners", "Mumbai", "India", "INR 65 LPA", endpoint, DateTimeOffset.UtcNow.AddHours(-12))
        ];
    }
}

public class GenericNewsEventTenderAdapter(ILogger<GenericNewsEventTenderAdapter> logger, IHttpClientFactory httpClientFactory) : AdapterBase(logger, httpClientFactory), ISourceAdapter
{
    public string Family => "signal-feed";

    public Task<IReadOnlyList<IngestedOpportunity>> FetchAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        logger.LogInformation("AdapterSignalFeedNoop | Family={Family}", Family);
        return Task.FromResult<IReadOnlyList<IngestedOpportunity>>([]);
    }
}
