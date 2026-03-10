namespace SentinelCareer.Contracts.Ingestion;

public record IngestedOpportunity(string ExternalId, string Title, string Description, string Company, string Location, string Country, string? Compensation, string Url, DateTimeOffset PostedAt);
public interface ISourceAdapter
{
    string Family { get; }
    Task<IReadOnlyList<IngestedOpportunity>> FetchAsync(CancellationToken cancellationToken);
}
