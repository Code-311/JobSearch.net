using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SentinelCareer.Application.Abstractions;
using SentinelCareer.Domain.Entities;
using SentinelCareer.Domain.Enums;
using SentinelCareer.Domain.Rules;

namespace SentinelCareer.Web.Pages.Opportunities;

public class IndexModel(IOpportunityRepository repository, IScoreRepository scoreRepository) : PageModel
{
    [BindProperty(SupportsGet = true)] public string? Location { get; set; }
    [BindProperty(SupportsGet = true)] public string? Company { get; set; }
    [BindProperty(SupportsGet = true)] public int? MinScore { get; set; }

    public IReadOnlyList<JobOpportunity> Items { get; private set; } = [];
    public Dictionary<Guid, IReadOnlyList<OpportunityScoreHistory>> Histories { get; private set; } = new();

    public async Task OnGet(CancellationToken cancellationToken)
    {
        var items = await repository.ListAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(Location))
            items = items.Where(x => x.Location?.City.Contains(Location, StringComparison.OrdinalIgnoreCase) == true).ToList();

        if (!string.IsNullOrWhiteSpace(Company))
            items = items.Where(x => x.Company?.Name.Contains(Company, StringComparison.OrdinalIgnoreCase) == true).ToList();

        if (MinScore.HasValue)
            items = items.Where(x => (x.LatestScore?.CompositeScore ?? 0) >= MinScore.Value).ToList();

        Items = items;
        foreach (var item in Items.Take(20))
            Histories[item.Id] = await scoreRepository.GetHistoryAsync(item.Id, cancellationToken);
    }

    public async Task<IActionResult> OnPostDecisionAsync(Guid id, string decision, string? notes, CancellationToken cancellationToken)
    {
        var status = decision.ToLowerInvariant() switch
        {
            "pursue" => OpportunityStatus.Shortlisted,
            "review" => OpportunityStatus.Tracked,
            "ignore" => OpportunityStatus.Ignored,
            _ => OpportunityStatus.Tracked
        };

        await repository.UpdateReviewAsync(id, status, notes ?? string.Empty, cancellationToken);
        return RedirectToPage(new { Location, Company, MinScore });
    }

    public static string ScoreBand(JobOpportunity item)
        => ScoreBandClassifier.Classify(item.LatestScore?.CompositeScore ?? 0).ToString();
}
