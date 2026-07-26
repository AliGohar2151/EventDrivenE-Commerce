using ECommerce.Application.Services;
using ECommerce.Contracts.Authentication;
using ECommerce.Infrastructure.Authentication;
using ECommerce.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace ECommerce.UnitTests.Authentication;

public class AuthenticationServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtProvider _jwtProvider;
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _passwordHasher = new PasswordHasher();

        var jwtOptions = Options.Create(new JwtOptions
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            SecretKey = "SuperSecretKeyForEventDrivenECommerceBackendSecuritySystemMustBeLongEnough!",
            AccessTokenExpirationMinutes = 15
        });
        _jwtProvider = new JwtProvider(jwtOptions);

        _service = new AuthenticationService(_context, _passwordHasher, _jwtProvider);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUserAndReturnTokens()
    {
        var request = new RegisterUserRequest("newuser@test.com", "Password123!", "Jane", "Doe");

        var result = await _service.RegisterAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
        result.Value.User.Email.Should().Be("newuser@test.com");
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ShouldReturnConflictFailure()
    {
        var request = new RegisterUserRequest("duplicate@test.com", "Password123!", "Jane", "Doe");

        await _service.RegisterAsync(request);
        var result = await _service.RegisterAsync(request);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.EmailAlreadyExists");
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ShouldReturnTokens()
    {
        var registerRequest = new RegisterUserRequest("loginuser@test.com", "Password123!", "Jane", "Doe");
        await _service.RegisterAsync(registerRequest);

        var loginRequest = new LoginRequest("loginuser@test.com", "Password123!");
        var result = await _service.LoginAsync(loginRequest);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ShouldReturnUnauthorizedFailure()
    {
        var registerRequest = new RegisterUserRequest("wrongpass@test.com", "Password123!", "Jane", "Doe");
        await _service.RegisterAsync(registerRequest);

        var loginRequest = new LoginRequest("wrongpass@test.com", "WrongPassword!");
        var result = await _service.LoginAsync(loginRequest);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Auth.InvalidCredentials");
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidToken_ShouldRotateRefreshToken()
    {
        var registerRequest = new RegisterUserRequest("refreshtest@test.com", "Password123!", "Jane", "Doe");
        var regResult = await _service.RegisterAsync(registerRequest);

        var refreshRequest = new RefreshTokenRequest(regResult.Value.RefreshToken);
        var refreshResult = await _service.RefreshTokenAsync(refreshRequest);

        refreshResult.IsSuccess.Should().BeTrue();
        refreshResult.Value.RefreshToken.Should().NotBe(regResult.Value.RefreshToken);
    }
}
