namespace SentinelCareer.Contracts.Notifications;

public interface INotificationDispatcher
{
    Task NotifyDesktopAsync(string title, string body, CancellationToken cancellationToken);
}
