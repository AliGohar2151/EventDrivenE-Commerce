namespace ECommerce.Contracts.Authentication;

public record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions
);
