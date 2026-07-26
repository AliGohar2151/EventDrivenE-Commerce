using ECommerce.Infrastructure.Authentication;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Authentication;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void HashPassword_ShouldReturnHashedString()
    {
        var password = "SecurePassword123!";

        var hash = _hasher.HashPassword(password);

        hash.Should().NotBeNullOrEmpty();
        hash.Should().Contain(".");
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        var password = "SecurePassword123!";
        var hash = _hasher.HashPassword(password);

        var result = _hasher.VerifyPassword(password, hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
    {
        var password = "SecurePassword123!";
        var hash = _hasher.HashPassword(password);

        var result = _hasher.VerifyPassword("WrongPassword!", hash);

        result.Should().BeFalse();
    }
}
