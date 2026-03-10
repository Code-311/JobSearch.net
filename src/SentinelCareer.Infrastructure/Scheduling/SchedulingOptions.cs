namespace SentinelCareer.Infrastructure.Scheduling;

public class SchedulingOptions
{
    public const string Section = "Scheduling";
    public int Tier1Hours { get; set; } = 6;
    public int Tier2Hours { get; set; } = 24;
    public int SignalsHours { get; set; } = 24;
    public int EnrichmentHours { get; set; } = 24;
    public int ReconciliationHours { get; set; } = 24;
}
