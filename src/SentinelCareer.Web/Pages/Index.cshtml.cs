using Microsoft.AspNetCore.Mvc.RazorPages;
using SentinelCareer.Application.Abstractions;

namespace SentinelCareer.Web.Pages;

public class IndexModel(IOpportunityRepository repository) : PageModel
{
    public int Total { get; private set; }
    public int HighPriority { get; private set; }
    public int NewHighPriority { get; private set; }
    public int ManualReview { get; private set; }
    public int TrackOnly { get; private set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        var items = await repository.ListAsync(cancellationToken);
        Total = items.Count;
        HighPriority = items.Count(x => (x.LatestScore?.CompositeScore ?? 0) >= 80);
        NewHighPriority = items.Count(x => (x.LatestScore?.CompositeScore ?? 0) >= 80 && x.PublishedAt >= DateTimeOffset.UtcNow.AddDays(-1));
        ManualReview = items.Count(x => (x.LatestScore?.CompositeScore ?? 0) is >= 65 and < 80);
        TrackOnly = items.Count(x => (x.LatestScore?.CompositeScore ?? 0) < 65);
    }
}
