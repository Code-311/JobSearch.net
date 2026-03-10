using System.Diagnostics;
using System.Runtime.InteropServices;
using SentinelCareer.Contracts.Notifications;

namespace SentinelCareer.Infrastructure.Notifications;

public class WindowsDesktopNotificationDispatcher(ILogger<WindowsDesktopNotificationDispatcher> logger) : INotificationDispatcher
{
    public async Task NotifyDesktopAsync(string title, string body, CancellationToken cancellationToken)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            logger.LogInformation("DesktopNotification(non-windows) | {Title} | {Body}", title, body);
            return;
        }

        try
        {
            var escapedTitle = title.Replace("'", "''");
            var escapedBody = body.Replace("'", "''");
            var script = "$ErrorActionPreference='Stop';"
                         + "[Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType=WindowsRuntime] > $null;"
                         + "$template=[Windows.UI.Notifications.ToastTemplateType]::ToastText02;"
                         + "$xml=[Windows.UI.Notifications.ToastNotificationManager]::GetTemplateContent($template);"
                         + "$nodes=$xml.GetElementsByTagName('text');"
                         + "$nodes[0].AppendChild($xml.CreateTextNode('" + escapedTitle + "')) > $null;"
                         + "$nodes[1].AppendChild($xml.CreateTextNode('" + escapedBody + "')) > $null;"
                         + "$toast=[Windows.UI.Notifications.ToastNotification]::new($xml);"
                         + "$notifier=[Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier('SentinelCareer');"
                         + "$notifier.Show($toast);";

            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            });

            if (process is null)
            {
                logger.LogWarning("DesktopNotification powershell start failed | {Title}", title);
                return;
            }

            await process.WaitForExitAsync(cancellationToken);
            if (process.ExitCode != 0)
            {
                var err = await process.StandardError.ReadToEndAsync(cancellationToken);
                logger.LogWarning("DesktopNotification powershell failed ({Code}) | {Error}", process.ExitCode, err);
            }
            else
            {
                logger.LogInformation("DesktopNotification sent | {Title}", title);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "DesktopNotification exception | {Title}", title);
        }
    }
}
