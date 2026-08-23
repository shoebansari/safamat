using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.TenantPanel;
using Matrimonial.AdminApi.DTOs.UserPanel;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/user/auth")]
public class UserAuthController : ControllerBase
{
    private readonly IUserAuthService _service;
    private readonly IUserSubscriptionService _subscriptionService;

    public UserAuthController(IUserAuthService service, IUserSubscriptionService subscriptionService)
    {
        _service = service;
        _subscriptionService = subscriptionService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserLoginResponse>>> Register([FromBody] UserRegisterRequest request)
    {
        var (result, error) = await _service.RegisterAsync(request);
        if (result == null) return BadRequest(ApiResponse<UserLoginResponse>.Fail(error ?? "Registration failed."));
        return Ok(ApiResponse<UserLoginResponse>.Ok(result, "Registration successful."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<UserLoginResponse>>> Login([FromBody] UserLoginRequest request)
    {
        var (result, error) = await _service.LoginAsync(request);
        if (result == null) return Unauthorized(ApiResponse<UserLoginResponse>.Fail(error ?? "Login failed."));
        return Ok(ApiResponse<UserLoginResponse>.Ok(result, "Login successful."));
    }

    [HttpGet("plans")]
    public async Task<ActionResult<ApiResponse<List<MemberPlanDto>>>> GetPlans([FromQuery] string tenantCode)
    {
        if (string.IsNullOrWhiteSpace(tenantCode))
            return BadRequest(ApiResponse<List<MemberPlanDto>>.Fail("Tenant code is required."));

        var plans = await _subscriptionService.GetPlansByTenantCodeAsync(tenantCode);
        return Ok(ApiResponse<List<MemberPlanDto>>.Ok(plans));
    }
}
