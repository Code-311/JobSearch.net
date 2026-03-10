namespace SentinelCareer.Application.Services;

public class DedupeService
{
    public string BuildKey(string normalizedTitle, string normalizedCompany, string city)
        => $"{normalizedTitle}|{normalizedCompany}|{city.Trim().ToLowerInvariant()}";

    public bool IsLikelyDuplicate(DateTimeOffset firstPublished, DateTimeOffset candidatePublished)
        => Math.Abs((firstPublished - candidatePublished).TotalDays) <= 30;
}
