using ECommerce.Domain.Primitives;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Domain;

public class ValueObjectTests
{
    private class Address : ValueObject
    {
        public Address(string street, string city, string zipCode)
        {
            Street = street;
            City = city;
            ZipCode = zipCode;
        }

        public string Street { get; }
        public string City { get; }
        public string ZipCode { get; }

        public override IEnumerable<object?> GetAtomicValues()
        {
            yield return Street;
            yield return City;
            yield return ZipCode;
        }
    }

    [Fact]
    public void ValueObjectsWithSameValues_ShouldBeEqual()
    {
        var address1 = new Address("123 Main St", "Springfield", "12345");
        var address2 = new Address("123 Main St", "Springfield", "12345");

        (address1 == address2).Should().BeTrue();
        address1.Equals(address2).Should().BeTrue();
        address1.GetHashCode().Should().Be(address2.GetHashCode());
    }

    [Fact]
    public void ValueObjectsWithDifferentValues_ShouldNotBeEqual()
    {
        var address1 = new Address("123 Main St", "Springfield", "12345");
        var address2 = new Address("456 Elm St", "Springfield", "12345");

        (address1 == address2).Should().BeFalse();
        (address1 != address2).Should().BeTrue();
    }
}
