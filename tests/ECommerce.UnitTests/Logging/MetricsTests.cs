using ECommerce.Infrastructure.Logging;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Logging;

public class MetricsTests
{
    [Fact]
    public void ECommerceMetrics_ShouldInitializeCounters()
    {
        var metrics = new ECommerceMetrics();

        metrics.OrdersCreatedCounter.Should().NotBeNull();
        metrics.EventsPublishedCounter.Should().NotBeNull();
        metrics.EventsConsumedCounter.Should().NotBeNull();
        metrics.EventFailuresCounter.Should().NotBeNull();
        metrics.RetryAttemptsCounter.Should().NotBeNull();
        metrics.DeadLetterMessagesCounter.Should().NotBeNull();
        metrics.PaymentFailuresCounter.Should().NotBeNull();
        metrics.InventoryFailuresCounter.Should().NotBeNull();
    }
}
