using ECommerce.Contracts.Authentication;
using ECommerce.Domain.Primitives;

namespace ECommerce.Application.Abstractions;

public interface IAuthenticationService
{
    Task<Result<AuthenticationResponse>> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<Result> RevokeTokenAsync(RevokeTokenRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
