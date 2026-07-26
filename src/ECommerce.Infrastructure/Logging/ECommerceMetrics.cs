using System.Diagnostics.Metrics;

namespace ECommerce.Infrastructure.Logging;

public class ECommerceMetrics
{
    public const string MeterName = "ECommerce.Backend";

    private readonly Meter _meter;

    public Counter<long> OrdersCreatedCounter { get; }
    public Counter<long> EventsPublishedCounter { get; }
    public Counter<long> EventsConsumedCounter { get; }
    public Counter<long> EventFailuresCounter { get; }
    public Counter<long> RetryAttemptsCounter { get; }
    public Counter<long> DeadLetterMessagesCounter { get; }
    public Counter<long> PaymentFailuresCounter { get; }
    public Counter<long> InventoryFailuresCounter { get; }

    public ECommerceMetrics()
    {
        _meter = new Meter(MeterName, "1.0.0");

        OrdersCreatedCounter = _meter.CreateCounter<long>("ecommerce.orders.created", "orders", "Total orders created");
        EventsPublishedCounter = _meter.CreateCounter<long>("ecommerce.events.published", "events", "Total events published");
        EventsConsumedCounter = _meter.CreateCounter<long>("ecommerce.events.consumed", "events", "Total events consumed");
        EventFailuresCounter = _meter.CreateCounter<long>("ecommerce.events.failures", "failures", "Total event failures");
        RetryAttemptsCounter = _meter.CreateCounter<long>("ecommerce.events.retries", "retries", "Total retry attempts");
        DeadLetterMessagesCounter = _meter.CreateCounter<long>("ecommerce.events.deadletter", "messages", "Total dead-letter messages");
        PaymentFailuresCounter = _meter.CreateCounter<long>("ecommerce.payments.failures", "failures", "Total payment failures");
        InventoryFailuresCounter = _meter.CreateCounter<long>("ecommerce.inventory.failures", "failures", "Total inventory failures");
    }
}
