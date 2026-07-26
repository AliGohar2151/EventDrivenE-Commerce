using ECommerce.Contracts.Authentication;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions;

public interface IJwtProvider
{
    (string Token, DateTime ExpiresAtUtc) GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);
    string GenerateRefreshToken();
}
