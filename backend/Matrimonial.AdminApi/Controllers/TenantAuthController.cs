using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.TenantPanel;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/tenant/auth")]
public class TenantAuthController : ControllerBase
{
    private readonly ITenantAuthService _service;

    public TenantAuthController(ITenantAuthService service) => _service = service;

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<TenantLoginResponse>>> Login([FromBody] TenantLoginRequest request)
    {
        var (result, error) = await _service.LoginAsync(request);
        if (result == null)
            return Unauthorized(ApiResponse<TenantLoginResponse>.Fail(error ?? "Invalid username or password."));
        return Ok(ApiResponse<TenantLoginResponse>.Ok(result, "Login successful."));
    }
}
