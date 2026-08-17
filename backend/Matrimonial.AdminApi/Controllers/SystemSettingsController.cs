using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.SystemSettings;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SystemSettingsController : ControllerBase
{
    private readonly ISystemSettingService _service;

    public SystemSettingsController(ISystemSettingService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SystemSettingDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var result = await _service.GetAllAsync(page, pageSize, search);
        return Ok(ApiResponse<PagedResult<SystemSettingDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<SystemSettingDto>>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<SystemSettingDto>.Fail("Setting not found."));
        return Ok(ApiResponse<SystemSettingDto>.Ok(result));
    }

    [HttpGet("by-key/{key}")]
    public async Task<ActionResult<ApiResponse<SystemSettingDto>>> GetByKey(string key)
    {
        var result = await _service.GetByKeyAsync(key);
        if (result == null) return NotFound(ApiResponse<SystemSettingDto>.Fail("Setting not found."));
        return Ok(ApiResponse<SystemSettingDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<SystemSettingDto>>> Create([FromBody] CreateSystemSettingRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.SettingId }, ApiResponse<SystemSettingDto>.Ok(result, "Setting created."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<SystemSettingDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<SystemSettingDto>>> Update(Guid id, [FromBody] UpdateSystemSettingRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<SystemSettingDto>.Fail("Setting not found."));
        return Ok(ApiResponse<SystemSettingDto>.Ok(result, "Setting updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Setting not found."));
        return Ok(ApiResponse<object>.Ok(new { }, "Setting deleted."));
    }
}
