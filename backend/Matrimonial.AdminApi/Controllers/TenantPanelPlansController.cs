using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.TenantPanel;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/tenant/plans")]
[Authorize(Roles = "Tenant")]
public class TenantPanelPlansController : ControllerBase
{
    private readonly ITenantMemberPlanService _service;

    public TenantPanelPlansController(ITenantMemberPlanService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<MemberPlanDto>>>> GetAll()
    {
        var tenantId = TenantUserContext.GetTenantId(User);
        var result = await _service.GetAllAsync(tenantId);
        return Ok(ApiResponse<List<MemberPlanDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MemberPlanDto>>> Create([FromBody] CreateMemberPlanRequest request)
    {
        var tenantId = TenantUserContext.GetTenantId(User);
        var result = await _service.CreateAsync(tenantId, request);
        return Ok(ApiResponse<MemberPlanDto>.Ok(result, "Plan created."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<MemberPlanDto>>> Update(Guid id, [FromBody] UpdateMemberPlanRequest request)
    {
        var tenantId = TenantUserContext.GetTenantId(User);
        var result = await _service.UpdateAsync(tenantId, id, request);
        if (result == null) return NotFound(ApiResponse<MemberPlanDto>.Fail("Plan not found."));
        return Ok(ApiResponse<MemberPlanDto>.Ok(result, "Plan updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var tenantId = TenantUserContext.GetTenantId(User);
        var deleted = await _service.DeleteAsync(tenantId, id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Plan not found."));
        return Ok(ApiResponse<object>.Ok(new { }, "Plan deactivated."));
    }
}
