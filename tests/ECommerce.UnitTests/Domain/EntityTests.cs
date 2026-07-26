using ECommerce.Domain.Primitives;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Domain;

public class EntityTests
{
    private class TestEntity : Entity<Guid>
    {
        public TestEntity(Guid id) : base(id) { }
    }

    [Fact]
    public void EntitiesWithSameId_ShouldBeEqual()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        (entity1 == entity2).Should().BeTrue();
        entity1.Equals(entity2).Should().BeTrue();
    }

    [Fact]
    public void EntitiesWithDifferentIds_ShouldNotBeEqual()
    {
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        (entity1 == entity2).Should().BeFalse();
        (entity1 != entity2).Should().BeTrue();
    }
}
