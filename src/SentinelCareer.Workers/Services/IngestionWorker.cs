using SentinelCareer.Application.Abstractions;
using SentinelCareer.Application.Services;
using SentinelCareer.Contracts.Ingestion;
using SentinelCareer.Domain.Entities;

namespace SentinelCareer.Workers.Services;

public class IngestionWorker(IServiceProvider serviceProvider, ILogger<IngestionWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceProvider.CreateScope();
            var adapters = scope.ServiceProvider.GetServices<ISourceAdapter>();
            var repo = scope.ServiceProvider.GetRequiredService<IOpportunityRepository>();
            var sourceRepo = scope.ServiceProvider.GetRequiredService<ISourceRepository>();
            var normalize = scope.ServiceProvider.GetRequiredService<NormalizationService>();
            var dedupe = scope.ServiceProvider.GetRequiredService<DedupeService>();

            foreach (var adapter in adapters)
            {
                logger.LogInformation("SourceRunStart | {Family}", adapter.Family);
                var run = await sourceRepo.StartRunAsync(adapter.Family, stoppingToken);
                IReadOnlyList<IngestedOpportunity> items = [];
                var runOk = false;

                for (int attempt = 1; attempt <= 3 && !runOk; attempt++)
                {
                    try
                    {
                        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                        timeoutCts.CancelAfter(TimeSpan.FromSeconds(45));
                        items = await adapter.FetchAsync(timeoutCts.Token);
                        runOk = true;
                    }
                    catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
                    {
                        logger.LogWarning("SourceRunTimeout | {Family} | Attempt={Attempt}", adapter.Family, attempt);
                        if (attempt < 3)
                            await Task.Delay(TimeSpan.FromSeconds(8 * attempt + Random.Shared.Next(1, 4)), stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "SourceRunRetry | {Family} | Attempt={Attempt}", adapter.Family, attempt);
                        if (attempt < 3)
                            await Task.Delay(TimeSpan.FromSeconds(8 * attempt + Random.Shared.Next(1, 4)), stoppingToken);
                    }
                }

                if (!runOk)
                {
                    logger.LogError("SourceRunFailed | {Family}", adapter.Family);
                    await sourceRepo.CompleteRunAsync(run.Id, "Failed", $"Adapter failed after retries: {adapter.Family}", stoppingToken);
                    continue;
                }

                int saved = 0;
                foreach (var item in items)
                {
                    try
                    {
                        if (normalize.IsExcludedRole(item.Title, item.Description))
                        {
                            logger.LogInformation("OpportunityExcluded | {ExternalId} | {Title}", item.ExternalId, item.Title);
                            continue;
                        }

                        if (await repo.FindByExternalIdAsync(item.ExternalId, stoppingToken) is not null)
                            continue;

                        var normalizedTitle = normalize.NormalizeTitle(item.Title);
                        var normalizedCompany = normalize.NormalizeCompany(item.Company);
                        var existing = await repo.FindPotentialDuplicateAsync(normalizedTitle, normalizedCompany, item.Location.ToLowerInvariant(), item.PostedAt, stoppingToken);
                        if (existing is not null && dedupe.IsLikelyDuplicate(existing.PublishedAt, item.PostedAt))
                        {
                            logger.LogInformation("OpportunityDeduped | {ExternalId} -> {ExistingId}", item.ExternalId, existing.Id);
                            continue;
                        }

                        var opp = new JobOpportunity
                        {
                            ExternalId = item.ExternalId,
                            Title = item.Title,
                            NormalizedTitle = normalizedTitle,
                            Description = item.Description,
                            PublishedAt = item.PostedAt,
                            IsIndiaPriority = item.Country.Equals("India", StringComparison.OrdinalIgnoreCase),
                            Company = new Company { Name = item.Company, NormalizedName = normalizedCompany },
                            Location = new Location { City = item.Location, Region = item.Location, Country = item.Country }
                        };
                        await repo.UpsertAsync(opp, stoppingToken);
                        saved++;
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "OpportunityIngestionFailed | Source={Family} | ExternalId={ExternalId}", adapter.Family, item.ExternalId);
                    }
                }

                logger.LogInformation("SourceRunEnd | {Family} | Ingested={Count} | Saved={Saved}", adapter.Family, items.Count, saved);
                await sourceRepo.CompleteRunAsync(run.Id, "Completed", $"Ingested={items.Count};Saved={saved}", stoppingToken);
            }

            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
        }
    }
}
