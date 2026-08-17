using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.AdminUsers;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _service;

    public AdminUsersController(IAdminUserService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AdminUserDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var result = await _service.GetAllAsync(page, pageSize, search);
        return Ok(ApiResponse<PagedResult<AdminUserDto>>.Ok(result));
    }

    [HttpGet("exists")]
    public async Task<ActionResult<ApiResponse<bool>>> UsernameExists([FromQuery] string username)
    {
        var exists = await _service.UsernameExistsAsync(username);
        return Ok(ApiResponse<bool>.Ok(exists));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<AdminUserDto>>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<AdminUserDto>.Fail("Admin user not found."));
        return Ok(ApiResponse<AdminUserDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AdminUserDto>>> Create([FromBody] CreateAdminUserRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.AdminId }, ApiResponse<AdminUserDto>.Ok(result, "Admin user created."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<AdminUserDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<AdminUserDto>>> Update(Guid id, [FromBody] UpdateAdminUserRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<AdminUserDto>.Fail("Admin user not found."));
        return Ok(ApiResponse<AdminUserDto>.Ok(result, "Admin user updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Admin user not found."));
        return Ok(ApiResponse<object>.Ok(new { }, "Admin user deleted."));
    }
}
