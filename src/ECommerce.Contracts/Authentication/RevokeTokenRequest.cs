namespace ECommerce.Contracts.Authentication;

public record RevokeTokenRequest(
    string RefreshToken
);
