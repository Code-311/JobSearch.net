using SentinelCareer.Contracts.LLM;

namespace SentinelCareer.Infrastructure.Notifications;

public class DisabledLocalModelProvider : ILocalModelProvider
{
    public bool IsEnabled => false;
    public Task<string> RewriteExplanationAsync(string deterministicExplanation, CancellationToken cancellationToken) => Task.FromResult(deterministicExplanation);
    public Task<string> GenerateInterviewPackAsync(string prompt, CancellationToken cancellationToken) => Task.FromResult(prompt);
}
