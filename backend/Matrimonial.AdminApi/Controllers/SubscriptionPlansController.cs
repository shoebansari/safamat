using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.SubscriptionPlans;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SubscriptionPlansController : ControllerBase
{
    private readonly ISubscriptionPlanService _service;

    public SubscriptionPlansController(ISubscriptionPlanService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SubscriptionPlanDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool? isActive = null)
    {
        var result = await _service.GetAllAsync(page, pageSize, isActive);
        return Ok(ApiResponse<PagedResult<SubscriptionPlanDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<SubscriptionPlanDto>>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<SubscriptionPlanDto>.Fail("Plan not found."));
        return Ok(ApiResponse<SubscriptionPlanDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<SubscriptionPlanDto>>> Create([FromBody] CreateSubscriptionPlanRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.PlanId }, ApiResponse<SubscriptionPlanDto>.Ok(result, "Plan created."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<SubscriptionPlanDto>>> Update(Guid id, [FromBody] UpdateSubscriptionPlanRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<SubscriptionPlanDto>.Fail("Plan not found."));
        return Ok(ApiResponse<SubscriptionPlanDto>.Ok(result, "Plan updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Plan not found."));
        return Ok(ApiResponse<object>.Ok(new { }, "Plan deleted."));
    }
}
