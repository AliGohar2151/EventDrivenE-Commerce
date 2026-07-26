using ECommerce.Domain.Primitives;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Domain;

public class AggregateRootTests
{
    private record TestDomainEvent(Guid Id, DateTime OccurredOnUtc) : IDomainEvent;

    private class TestAggregate : AggregateRoot<Guid>
    {
        public TestAggregate(Guid id) : base(id) { }

        public void DoSomething()
        {
            AddDomainEvent(new TestDomainEvent(Guid.NewGuid(), DateTime.UtcNow));
        }
    }

    [Fact]
    public void AddDomainEvent_ShouldRecordEvent()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());

        aggregate.DoSomething();

        aggregate.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        var aggregate = new TestAggregate(Guid.NewGuid());
        aggregate.DoSomething();

        aggregate.ClearDomainEvents();

        aggregate.DomainEvents.Should().BeEmpty();
    }
}
