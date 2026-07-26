using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class Role : Entity<Guid>
{
    private Role(Guid id, string name, string description)
        : base(id)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }

    public static Role Create(string name, string description)
    {
        return new Role(Guid.NewGuid(), name, description);
    }
}
