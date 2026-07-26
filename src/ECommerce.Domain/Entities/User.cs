using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class User : Entity<Guid>
{
    private User(Guid id, string email, string firstName, string lastName, bool isActive)
        : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        IsActive = isActive;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

    public static User Create(string email, string firstName, string lastName)
    {
        return new User(Guid.NewGuid(), email, firstName, lastName, true);
    }
}
