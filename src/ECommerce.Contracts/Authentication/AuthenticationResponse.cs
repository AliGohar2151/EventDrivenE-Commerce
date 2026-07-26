namespace ECommerce.Contracts.Authentication;

public record AuthenticationResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    UserResponse User
);
