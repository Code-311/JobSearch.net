using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SentinelCareer.Application.Abstractions;
using SentinelCareer.Application.Options;
using SentinelCareer.Application.Services;
using SentinelCareer.Contracts.Ingestion;
using SentinelCareer.Contracts.LLM;
using SentinelCareer.Contracts.Notifications;
using SentinelCareer.Infrastructure.Adapters;
using SentinelCareer.Infrastructure.Data;
using SentinelCareer.Infrastructure.Notifications;
using SentinelCareer.Infrastructure.Repositories;
using SentinelCareer.Infrastructure.Scheduling;

namespace SentinelCareer.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSentinelCareer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ScoringOptions>(configuration.GetSection(ScoringOptions.Section));
        services.Configure<SchedulingOptions>(configuration.GetSection(SchedulingOptions.Section));

        services.AddDbContext<SentinelCareerDbContext>(o =>
            o.UseNpgsql(configuration.GetConnectionString("Postgres") ?? "Host=localhost;Database=sentinelcareer;Username=postgres;Password=postgres"));

        services.AddHttpClient("sources");

        services.AddScoped<IOpportunityRepository, OpportunityRepository>();
        services.AddScoped<ISourceRepository, SourceRepository>();
        services.AddScoped<IScoreRepository, ScoreRepository>();

        services.AddSingleton<NormalizationService>();
        services.AddSingleton<DedupeService>();
        services.AddScoped(provider => new ScoringEngine(provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ScoringOptions>>().Value));
        services.AddScoped<InterviewPrepService>();

        services.AddSingleton<ILocalModelProvider, DisabledLocalModelProvider>();
        services.AddSingleton<INotificationDispatcher, WindowsDesktopNotificationDispatcher>();

        services.AddTransient<ISourceAdapter, GenericCompanyCareersAdapter>();
        services.AddTransient<ISourceAdapter, GenericExecutiveSearchAdapter>();
        services.AddTransient<ISourceAdapter, GenericNewsEventTenderAdapter>();
        return services;
    }
}
