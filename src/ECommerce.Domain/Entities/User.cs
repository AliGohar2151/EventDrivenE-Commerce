using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class User : Entity<Guid>
{
    private readonly List<RefreshToken> _refreshTokens = new();

    private User(Guid id, string email, string firstName, string lastName, string passwordHash, bool isActive)
        : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
        IsActive = isActive;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public static User Create(string email, string firstName, string lastName, string passwordHash)
    {
        return new User(Guid.NewGuid(), email, firstName, lastName, passwordHash, true);
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        _refreshTokens.Add(refreshToken);
    }
}
