namespace SentinelCareer.Contracts.LLM;

public interface ILocalModelProvider
{
    bool IsEnabled { get; }
    Task<string> RewriteExplanationAsync(string deterministicExplanation, CancellationToken cancellationToken);
    Task<string> GenerateInterviewPackAsync(string prompt, CancellationToken cancellationToken);
}
