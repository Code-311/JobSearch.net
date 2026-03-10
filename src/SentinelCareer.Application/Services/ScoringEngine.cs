using SentinelCareer.Application.Options;
using SentinelCareer.Contracts.Scoring;
using SentinelCareer.Domain.Entities;

namespace SentinelCareer.Application.Services;

public class ScoringEngine
{
    private readonly ScoringOptions _options;
    public ScoringEngine(ScoringOptions options) => _options = options;

    public ScoredOpportunity Score(JobOpportunity opportunity, decimal? maxCompInr, decimal sourceCredibility, int freshnessDays, int opportunityPotential)
    {
        List<string> reasons = [];
        string blob = $"{opportunity.Title} {opportunity.Description}";
        bool hasTargetIndustry = ContainsAny(blob, "defence", "aerospace", "strategic", "manufacturing", "industrial", "infrastructure", "logistics", "consulting", "audit", "governance", "compliance", "risk", "transformation");
        bool seniorRole = ContainsAny(opportunity.Title, "director", "vp", "avp", "head", "gm", "board", "advisor", "consultant");
        bool midLevelCue = ContainsAny(opportunity.Title, "manager", "lead", "specialist", "associate") && !seniorRole;

        int profileFit = ContainsAny(blob, "operations", "governance", "risk", "compliance", "transformation", "pmo", "execution") ? 90 : 55;
        int compensationFit;
        if (maxCompInr is null)
            compensationFit = seniorRole ? 58 : 45;
        else if (maxCompInr >= _options.AspirationalCompInrLpa * 100000)
            compensationFit = 95;
        else if (maxCompInr >= _options.BaselineCompInrLpa * 100000)
            compensationFit = 75;
        else
            compensationFit = 42;

        int geographyFit = opportunity.IsIndiaPriority ? 95 : 50;
        int seniorityFit = seniorRole ? 92 : 38;
        int industryFit = hasTargetIndustry ? 88 : 42;

        if (midLevelCue)
        {
            seniorityFit = Math.Max(seniorityFit - 18, 0);
            profileFit = Math.Max(profileFit - 12, 0);
            reasons.Add("Mid-level cues detected; seniority fit reduced.");
        }

        if (!hasTargetIndustry && ContainsAny(blob, "operations"))
        {
            industryFit = Math.Max(industryFit - 10, 0);
            reasons.Add("Generic operations context outside target sectors; industry fit reduced.");
        }

        if (ContainsAny(blob, "software engineer", "developer", "soc analyst", "security analyst", "business development", "sales manager"))
        {
            profileFit = Math.Max(profileFit - 35, 0);
            seniorityFit = Math.Max(seniorityFit - 25, 0);
            industryFit = Math.Max(industryFit - 20, 0);
            reasons.Add("Exclusion keywords detected (engineering/analyst/sales-heavy), score penalized.");
        }

        reasons.Add(profileFit >= 80 ? "Role strongly aligns to the target leadership profile." : "Role has partial alignment to the target profile.");
        reasons.Add(maxCompInr is null
            ? "Compensation unavailable; confidence reduced but not disqualifying."
            : maxCompInr >= _options.AspirationalCompInrLpa * 100000 ? "Compensation is in aspirational band (50L+)."
            : maxCompInr >= _options.BaselineCompInrLpa * 100000 ? "Compensation meets baseline threshold (35L+)."
            : "Compensation appears below baseline expectation.");
        reasons.Add(opportunity.IsIndiaPriority ? "India-first geography preference is satisfied." : "Non-India role retained but deprioritized.");
        reasons.Add(seniorityFit >= 80 ? "Seniority appears in the target band." : "Seniority appears below preferred band.");
        reasons.Add(industryFit >= 80 ? "Industry/sector context matches focus clusters." : "Industry relevance is moderate/low for target sectors.");

        int sourceScore = (int)Math.Clamp(sourceCredibility * 100, 0, 100);
        int freshness = freshnessDays <= 2 ? 95 : freshnessDays <= 7 ? 75 : freshnessDays <= 14 ? 60 : 40;
        int oppPotential = Math.Clamp(opportunityPotential, 0, 100);

        var subs = new Dictionary<string, int>
        {
            ["profile_fit"] = profileFit,
            ["compensation_fit"] = compensationFit,
            ["geography_fit"] = geographyFit,
            ["seniority_fit"] = seniorityFit,
            ["industry_fit"] = industryFit,
            ["source_credibility"] = sourceScore,
            ["freshness"] = freshness,
            ["opportunity_potential"] = oppPotential
        };

        var w = _options.Weights;
        int composite = (int)Math.Round(
            profileFit * w.ProfileFit + compensationFit * w.CompensationFit + geographyFit * w.GeographyFit + seniorityFit * w.SeniorityFit +
            industryFit * w.IndustryFit + sourceScore * w.SourceCredibility + freshness * w.Freshness + oppPotential * w.OpportunityPotential);

        string action = composite >= 80 ? "Pursue immediately" : composite >= 65 ? "Review manually" : "Track only";
        string explanation = string.Join(" ", reasons) + $" Final composite: {composite}.";
        return new ScoredOpportunity(composite, subs, explanation, action);
    }

    private static bool ContainsAny(string value, params string[] tokens)
        => tokens.Any(t => value.Contains(t, StringComparison.OrdinalIgnoreCase));
}
