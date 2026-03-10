using SentinelCareer.Contracts.LLM;
using SentinelCareer.Domain.Entities;

namespace SentinelCareer.Application.Services;

public class InterviewPrepService
{
    private readonly ILocalModelProvider _modelProvider;

    public InterviewPrepService(ILocalModelProvider modelProvider) => _modelProvider = modelProvider;

    public async Task<InterviewPrepPack> GenerateAsync(JobOpportunity opportunity, CancellationToken cancellationToken)
    {
        var company = opportunity.Company?.Name ?? "target company";
        var deterministic = new InterviewPrepPack
        {
            OpportunityId = opportunity.Id,
            CompanyBrief = $"{company} appears to be hiring senior leadership capacity tied to execution discipline, governance rigor, and transformation outcomes.",
            RoleBrief = $"{opportunity.Title} is framed as a mandate role: align strategy-to-execution, reduce delivery risk, and establish measurable operating rhythm.",
            FitAnalysis = "Profile fit is strongest where the mandate combines operations leadership, risk/compliance governance, and enterprise transformation execution.",
            QuestionsAndAnswers = "Q1: How do you stabilize execution quickly? A1: Establish governance cadence, critical-metric baseline, and risk heatmap in first 30 days. Q2: How do you align leaders? A2: Clarify decision rights and enforce cross-functional operating reviews.",
            Entry3090Plan = "30 days: diagnose mandate, stakeholders, and risk backlog. 60 days: launch PMO/governance controls and close top execution gaps. 90 days: institutionalize KPI-led rhythm and leadership accountability model.",
            StakeholderMap = "Primary: CEO/BU head (mandate sponsor), CHRO (org alignment), CFO (cost/risk trade-offs). Secondary: plant/ops heads, compliance/legal, key external regulators or customers.",
            KeyRisks = "Risk of mandate ambiguity, fragmented decision rights, underpowered transformation office, and compensation-to-scope mismatch.",
            NegotiationNotes = "Negotiate for explicit success metrics, authority boundaries, team shape, reporting line, and fixed-pay alignment to 35L baseline / 50L+ premium expectations."
        };

        if (!_modelProvider.IsEnabled) return deterministic;
        deterministic.RoleBrief = await _modelProvider.GenerateInterviewPackAsync(deterministic.RoleBrief, cancellationToken);
        return deterministic;
    }
}
