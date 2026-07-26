using Microsoft.AspNetCore.Authorization;

namespace ECommerce.Infrastructure.Authentication;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base(policy: permission)
    {
    }
}
