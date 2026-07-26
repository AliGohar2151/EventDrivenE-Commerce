using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class Permission : Entity<Guid>
{
    private Permission(Guid id, string name)
        : base(id)
    {
        Name = name;
    }

    public string Name { get; private set; }

    public static Permission Create(string name)
    {
        return new Permission(Guid.NewGuid(), name);
    }
}
