using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.TenantSubscriptions;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class TenantSubscriptionsController : ControllerBase
{
    private readonly ITenantSubscriptionService _service;

    public TenantSubscriptionsController(ITenantSubscriptionService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TenantSubscriptionDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] Guid? tenantId = null, [FromQuery] string? status = null)
    {
        var result = await _service.GetAllAsync(page, pageSize, tenantId, status);
        return Ok(ApiResponse<PagedResult<TenantSubscriptionDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TenantSubscriptionDto>>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<TenantSubscriptionDto>.Fail("Subscription not found."));
        return Ok(ApiResponse<TenantSubscriptionDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TenantSubscriptionDto>>> Create([FromBody] CreateTenantSubscriptionRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.TenantSubscriptionsId }, ApiResponse<TenantSubscriptionDto>.Ok(result, "Subscription created."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<TenantSubscriptionDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TenantSubscriptionDto>>> Update(Guid id, [FromBody] UpdateTenantSubscriptionRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<TenantSubscriptionDto>.Fail("Subscription not found."));
        return Ok(ApiResponse<TenantSubscriptionDto>.Ok(result, "Subscription updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Subscription not found."));
        return Ok(ApiResponse<object>.Ok(new { }, "Subscription deleted."));
    }
}
