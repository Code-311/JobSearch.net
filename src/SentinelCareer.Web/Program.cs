using Microsoft.EntityFrameworkCore;
using SentinelCareer.Application.Abstractions;
using SentinelCareer.Infrastructure;
using SentinelCareer.Infrastructure.Data;
using SentinelCareer.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSentinelCareer(builder.Configuration);
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SentinelCareerDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.EnsureAsync(db, CancellationToken.None);
}

app.UseStaticFiles();
app.MapRazorPages();
app.MapGet("/api/opportunities", async (IOpportunityRepository repo, CancellationToken ct) => await repo.ListAsync(ct));

app.Run();

public partial class Program { }
