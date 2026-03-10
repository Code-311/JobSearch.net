using System.Text.RegularExpressions;
using SentinelCareer.Contracts.Ingestion;

namespace SentinelCareer.Infrastructure.Adapters;

public static class SourceHtmlParser
{
    public static IReadOnlyList<IngestedOpportunity> ParseSimpleCards(string html, string companyHint, string endpoint, Action<string>? log = null)
    {
        List<IngestedOpportunity> results = [];
        if (string.IsNullOrWhiteSpace(html)) return results;

        var cards = Regex.Matches(html, "<(?<tag>div|article)[^>]*(class|data-role)=\"[^\"]*(job|career|posting)[^\"]*\"[^>]*>(?<body>.*?)</\\k<tag>>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        foreach (Match card in cards)
        {
            var chunk = card.Groups["body"].Value;
            var title = Extract(chunk, "title")
                        ?? ExtractTag(chunk, "h1|h2|h3")
                        ?? ExtractFallback(chunk, "director|vp|avp|head|gm|consultant|advisor");
            if (string.IsNullOrWhiteSpace(title))
            {
                log?.Invoke("ParserSkipCard | reason=no-title");
                continue;
            }

            var location = Extract(chunk, "location") ?? ExtractLabel(chunk, "location") ?? "India";
            var compensation = Extract(chunk, "comp|salary|ctc") ?? ExtractLabel(chunk, "salary|ctc|comp") ?? "Unknown";
            var externalId = ExtractAttribute(card.Value, "data-id") ?? $"{companyHint}-{Math.Abs((title + location).GetHashCode())}";

            results.Add(new IngestedOpportunity(
                externalId.Trim(),
                Cleanup(title),
                Cleanup(chunk),
                companyHint.Trim(),
                Cleanup(location),
                location.Contains("india", StringComparison.OrdinalIgnoreCase) ? "India" : "Unknown",
                Cleanup(compensation),
                endpoint.Trim(),
                DateTimeOffset.UtcNow));
        }

        log?.Invoke($"ParserResult | cards={cards.Count} parsed={results.Count}");
        return results;
    }

    private static string Cleanup(string raw) => Regex.Replace(raw, "\\s+", " ").Trim();

    private static string? Extract(string html, string clsPattern)
    {
        var m = Regex.Match(html, $"class=\"[^\"]*({clsPattern})[^\"]*\"[^>]*>([^<]+)", RegexOptions.IgnoreCase);
        return m.Success ? m.Groups[2].Value : null;
    }

    private static string? ExtractTag(string html, string tags)
    {
        var m = Regex.Match(html, $"<({tags})[^>]*>([^<]+)</\\1>", RegexOptions.IgnoreCase);
        return m.Success ? m.Groups[2].Value : null;
    }

    private static string? ExtractLabel(string html, string label)
    {
        var m = Regex.Match(html, $"({label})\\s*[:|-]\\s*([^<\\n]+)", RegexOptions.IgnoreCase);
        return m.Success ? m.Groups[2].Value : null;
    }

    private static string? ExtractFallback(string html, string pattern)
    {
        var m = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
        return m.Success ? m.Value : null;
    }

    private static string? ExtractAttribute(string html, string attribute)
    {
        var m = Regex.Match(html, $"{attribute}=\"([^\"]+)\"", RegexOptions.IgnoreCase);
        return m.Success ? m.Groups[1].Value : null;
    }
}
