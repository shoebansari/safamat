using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.Auth;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null)
            return Unauthorized(ApiResponse<LoginResponse>.Fail("Invalid username or password."));

        return Ok(ApiResponse<LoginResponse>.Ok(result, "Login successful."));
    }
}
