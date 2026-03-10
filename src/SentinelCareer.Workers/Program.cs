using SentinelCareer.Infrastructure;
using SentinelCareer.Workers.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSentinelCareer(builder.Configuration);
builder.Services.AddHostedService<IngestionWorker>();
builder.Services.AddHostedService<ScoringWorker>();
builder.Services.AddWindowsService();

var host = builder.Build();
await host.RunAsync();
