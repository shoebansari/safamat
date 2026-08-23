namespace Matrimonial.AdminApi.Common;

using System.Security.Claims;

public static class UserContext
{
    public static Guid GetUserId(ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("user_id");
        return Guid.Parse(id ?? throw new UnauthorizedAccessException("User not authenticated."));
    }

    public static Guid GetTenantId(ClaimsPrincipal user)
    {
        var id = user.FindFirstValue("tenant_id");
        return Guid.Parse(id ?? throw new UnauthorizedAccessException("Tenant not found."));
    }
}
