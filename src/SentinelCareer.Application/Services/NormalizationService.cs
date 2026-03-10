using System.Globalization;
using System.Text.RegularExpressions;

namespace SentinelCareer.Application.Services;

public class NormalizationService
{
    public string NormalizeTitle(string title)
        => title.Trim().ToLowerInvariant().Replace("director-level", "director").Replace("&", "and");

    public string NormalizeCompany(string company)
        => company.Trim().ToLowerInvariant().Replace("limited", "ltd").Replace("private", "pvt");

    public string NormalizeLocation(string city, string country)
        => $"{city.Trim()}, {country.Trim()}";

    public bool IsExcludedRole(string title, string description)
    {
        string blob = $"{title} {description}".ToLowerInvariant();
        string[] excluded = ["software engineer", "developer", "soc analyst", "security analyst", "inside sales", "business development", "sales manager"];
        return excluded.Any(blob.Contains);
    }

    public decimal? NormalizeCompensationToInr(string? compensationText, IReadOnlyDictionary<string, decimal> fx)
    {
        if (string.IsNullOrWhiteSpace(compensationText)) return null;

        var lower = compensationText.ToLowerInvariant().Replace(",", "").Trim();
        var numbers = Regex.Matches(lower, "\\d+(?:\\.\\d+)?").Select(m => decimal.Parse(m.Value, CultureInfo.InvariantCulture)).ToList();
        if (numbers.Count == 0) return null;

        decimal amount = numbers.Max();
        if (lower.Contains("cr") || lower.Contains("crore")) amount *= 10000000;
        else if (lower.Contains("lpa") || lower.Contains("lakh")) amount *= 100000;

        if ((lower.Contains("usd") || lower.Contains("$")) && fx.TryGetValue("USD", out var usdRate)) return amount * usdRate;
        if ((lower.Contains("eur") || lower.Contains("€")) && fx.TryGetValue("EUR", out var eurRate)) return amount * eurRate;
        if ((lower.Contains("gbp") || lower.Contains("£")) && fx.TryGetValue("GBP", out var gbpRate)) return amount * gbpRate;

        return amount;
    }
}
