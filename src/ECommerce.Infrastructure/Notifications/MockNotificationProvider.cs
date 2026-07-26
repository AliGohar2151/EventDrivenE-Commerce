using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Notifications;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Notifications;

public class MockNotificationProvider : INotificationProvider
{
    private readonly ILogger<MockNotificationProvider> _logger;

    public MockNotificationProvider(ILogger<MockNotificationProvider> logger)
    {
        _logger = logger;
    }

    public Task<NotificationProviderResult> SendAsync(
        string recipient,
        string subject,
        string body,
        NotificationChannel channel,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK NOTIFICATION PROVIDER] [{Channel}] Sent to '{Recipient}': Subject='{Subject}' | Body='{Body}'",
            channel, recipient, subject, body);

        return Task.FromResult(new NotificationProviderResult(true, null));
    }
}
