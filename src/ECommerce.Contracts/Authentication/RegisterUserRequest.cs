namespace ECommerce.Contracts.Authentication;

public record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
);
