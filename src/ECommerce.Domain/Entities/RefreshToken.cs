using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class RefreshToken : Entity<Guid>
{
    private RefreshToken(Guid id, Guid userId, string token, DateTime expiresOnUtc, DateTime createdOnUtc)
        : base(id)
    {
        UserId = userId;
        Token = token;
        ExpiresOnUtc = expiresOnUtc;
        CreatedOnUtc = createdOnUtc;
    }

    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresOnUtc { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? RevokedOnUtc { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresOnUtc;
    public bool IsRevoked => RevokedOnUtc.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    public static RefreshToken Create(Guid userId, string token, TimeSpan duration)
    {
        var now = DateTime.UtcNow;
        return new RefreshToken(Guid.NewGuid(), userId, token, now.Add(duration), now);
    }

    public void Revoke(string? replacedByToken = null)
    {
        RevokedOnUtc = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
    }
}
