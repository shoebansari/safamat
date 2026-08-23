using System.Security.Claims;

namespace Matrimonial.AdminApi.Common;

public static class TenantUserContext
{
    public static Guid GetTenantId(ClaimsPrincipal user)
    {
        var id = user.FindFirstValue("tenant_id") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var tenantId))
            throw new UnauthorizedAccessException("Invalid tenant session.");
        return tenantId;
    }
}
