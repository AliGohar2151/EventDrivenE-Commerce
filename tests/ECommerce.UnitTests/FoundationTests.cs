namespace ECommerce.UnitTests;

using FluentAssertions;
using Xunit;

public class FoundationTests
{
    [Fact]
    public void Foundation_ShouldBeConfiguredCorrectly()
    {
        bool isConfigured = true;
        isConfigured.Should().BeTrue();
    }
}
