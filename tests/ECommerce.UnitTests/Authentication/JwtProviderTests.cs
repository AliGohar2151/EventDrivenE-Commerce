using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Authentication;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace ECommerce.UnitTests.Authentication;

public class JwtProviderTests
{
    private readonly JwtProvider _provider;

    public JwtProviderTests()
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            SecretKey = "SuperSecretKeyForEventDrivenECommerceBackendSecuritySystemMustBeLongEnough!",
            AccessTokenExpirationMinutes = 15
        });

        _provider = new JwtProvider(options);
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnValidTokenAndExpiration()
    {
        var user = User.Create("user@test.com", "John", "Doe", "Hash");
        var roles = new[] { "Admin" };
        var permissions = new[] { "Users.Read" };

        var (token, expiresAtUtc) = _provider.GenerateAccessToken(user, roles, permissions);

        token.Should().NotBeNullOrEmpty();
        expiresAtUtc.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnRandomString()
    {
        var token1 = _provider.GenerateRefreshToken();
        var token2 = _provider.GenerateRefreshToken();

        token1.Should().NotBeNullOrEmpty();
        token2.Should().NotBeNullOrEmpty();
        token1.Should().NotBe(token2);
    }
}
