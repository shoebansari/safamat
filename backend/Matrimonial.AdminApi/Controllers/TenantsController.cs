using System.Security.Claims;
using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.Tenants;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _service;

    public TenantsController(ITenantService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TenantDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null, [FromQuery] bool? isActive = null)
    {
        var result = await _service.GetAllAsync(page, pageSize, search, isActive);
        return Ok(ApiResponse<PagedResult<TenantDto>>.Ok(result));
    }

    [HttpGet("exists")]
    public async Task<ActionResult<ApiResponse<object>>> Exists(
        [FromQuery] string? tenantCode = null, [FromQuery] string? companyName = null,
        [FromQuery] string? userName = null, [FromQuery] Guid? excludeTenantId = null)
    {
        var (codeExists, nameExists, userNameExists) =
            await _service.ExistsAsync(tenantCode, companyName, userName, excludeTenantId);
        return Ok(ApiResponse<object>.Ok(new
        {
            tenantCodeExists = codeExists,
            companyNameExists = nameExists,
            userNameExists = userNameExists
        }));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TenantDto>>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<TenantDto>.Fail("Tenant not found."));
        return Ok(ApiResponse<TenantDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TenantDto>>> Create([FromBody] CreateTenantRequest request)
    {
        try
        {
            var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.CreateAsync(request, adminId);
            return CreatedAtAction(nameof(GetById), new { id = result.TenantId }, ApiResponse<TenantDto>.Ok(result, "Tenant created."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<TenantDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TenantDto>>> Update(Guid id, [FromBody] UpdateTenantRequest request)
    {
        try
        {
            var result = await _service.UpdateAsync(id, request);
            if (result == null) return NotFound(ApiResponse<TenantDto>.Fail("Tenant not found."));
            return Ok(ApiResponse<TenantDto>.Ok(result, "Tenant updated."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<TenantDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Tenant not found."));
        return Ok(ApiResponse<object>.Ok(new { }, "Tenant deleted."));
    }
}
