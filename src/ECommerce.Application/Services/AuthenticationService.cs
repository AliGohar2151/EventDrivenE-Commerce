using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Authentication;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public AuthenticationService(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthenticationResponse>> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            return Result.Failure<AuthenticationResponse>(Error.Conflict("User.EmailAlreadyExists", "Email is already registered."));
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.Email, request.FirstName, request.LastName, passwordHash);

        _context.Users.Add(user);

        var (roles, permissions) = await GetUserRolesAndPermissionsAsync(user.Id, cancellationToken);
        var (accessToken, expiresAtUtc) = _jwtProvider.GenerateAccessToken(user, roles, permissions);

        var refreshTokenValue = _jwtProvider.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, refreshTokenValue, TimeSpan.FromDays(7));
        user.AddRefreshToken(refreshToken);
        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(cancellationToken);

        var userResponse = new UserResponse(user.Id, user.Email, user.FirstName, user.LastName, roles, permissions);
        return Result.Success(new AuthenticationResponse(accessToken, refreshTokenValue, expiresAtUtc, userResponse));
    }

    public async Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthenticationResponse>(Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password."));
        }

        if (!user.IsActive)
        {
            return Result.Failure<AuthenticationResponse>(Error.Forbidden("Auth.UserDeactivated", "User account is inactive."));
        }

        var (roles, permissions) = await GetUserRolesAndPermissionsAsync(user.Id, cancellationToken);
        var (accessToken, expiresAtUtc) = _jwtProvider.GenerateAccessToken(user, roles, permissions);

        var refreshTokenValue = _jwtProvider.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, refreshTokenValue, TimeSpan.FromDays(7));
        user.AddRefreshToken(refreshToken);
        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(cancellationToken);

        var userResponse = new UserResponse(user.Id, user.Email, user.FirstName, user.LastName, roles, permissions);
        return Result.Success(new AuthenticationResponse(accessToken, refreshTokenValue, expiresAtUtc, userResponse));
    }

    public async Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken is null || !refreshToken.IsActive)
        {
            return Result.Failure<AuthenticationResponse>(Error.Unauthorized("Auth.InvalidRefreshToken", "Invalid or expired refresh token."));
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == refreshToken.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Result.Failure<AuthenticationResponse>(Error.Unauthorized("Auth.UserNotFound", "User associated with token not found or inactive."));
        }

        var newRefreshTokenValue = _jwtProvider.GenerateRefreshToken();
        refreshToken.Revoke(newRefreshTokenValue);

        var newRefreshToken = RefreshToken.Create(user.Id, newRefreshTokenValue, TimeSpan.FromDays(7));
        _context.RefreshTokens.Add(newRefreshToken);

        var (roles, permissions) = await GetUserRolesAndPermissionsAsync(user.Id, cancellationToken);
        var (accessToken, expiresAtUtc) = _jwtProvider.GenerateAccessToken(user, roles, permissions);

        await _context.SaveChangesAsync(cancellationToken);

        var userResponse = new UserResponse(user.Id, user.Email, user.FirstName, user.LastName, roles, permissions);
        return Result.Success(new AuthenticationResponse(accessToken, newRefreshTokenValue, expiresAtUtc, userResponse));
    }

    public async Task<Result> RevokeTokenAsync(RevokeTokenRequest request, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken is null || !refreshToken.IsActive)
        {
            return Result.Failure(Error.NotFound("Auth.RefreshTokenNotFound", "Active refresh token not found."));
        }

        refreshToken.Revoke();
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<UserResponse>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<UserResponse>(Error.NotFound("User.NotFound", "User was not found."));
        }

        var (roles, permissions) = await GetUserRolesAndPermissionsAsync(user.Id, cancellationToken);
        return Result.Success(new UserResponse(user.Id, user.Email, user.FirstName, user.LastName, roles, permissions));
    }

    private async Task<(IReadOnlyCollection<string> Roles, IReadOnlyCollection<string> Permissions)> GetUserRolesAndPermissionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var roleIds = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);

        var roles = await _context.Roles
            .Where(r => roleIds.Contains(r.Id))
            .Select(r => r.Name)
            .ToListAsync(cancellationToken);

        var permissionIds = await _context.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.PermissionId)
            .ToListAsync(cancellationToken);

        var permissions = await _context.Permissions
            .Where(p => permissionIds.Contains(p.Id))
            .Select(p => p.Name)
            .ToListAsync(cancellationToken);

        return (roles.AsReadOnly(), permissions.AsReadOnly());
    }
}
