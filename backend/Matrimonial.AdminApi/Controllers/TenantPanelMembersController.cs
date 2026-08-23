using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.TenantPanel;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/tenant/members")]
[Authorize(Roles = "Tenant")]
public class TenantPanelMembersController : ControllerBase
{
    private readonly ITenantMemberService _service;

    public TenantPanelMembersController(ITenantMemberService service) => _service = service;

    [HttpGet("pending-approvals")]
    public async Task<ActionResult<ApiResponse<List<MemberDto>>>> GetPendingApprovals()
    {
        var tenantId = TenantUserContext.GetTenantId(User);
        var result = await _service.GetPendingApprovalsAsync(tenantId);
        return Ok(ApiResponse<List<MemberDto>>.Ok(result));
    }

    [HttpGet("{userCode}")]
    public async Task<ActionResult<ApiResponse<MemberDto>>> GetByUserCode(string userCode)
    {
        var tenantId = TenantUserContext.GetTenantId(User);
        var result = await _service.GetByUserCodeAsync(tenantId, userCode);
        if (result == null) return NotFound(ApiResponse<MemberDto>.Fail("User not found."));
        return Ok(ApiResponse<MemberDto>.Ok(result));
    }

    [HttpPut("{userCode}/plan")]
    public async Task<ActionResult<ApiResponse<MemberDto>>> UpdatePlan(
        string userCode, [FromBody] UpdateMemberPlanAssignmentRequest request)
    {
        try
        {
            var tenantId = TenantUserContext.GetTenantId(User);
            var result = await _service.UpdatePlanAssignmentAsync(tenantId, userCode, request);
            if (result == null) return NotFound(ApiResponse<MemberDto>.Fail("User not found."));
            return Ok(ApiResponse<MemberDto>.Ok(result, "Plan updated."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<MemberDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{memberId:guid}/profile-approval")]
    public async Task<ActionResult<ApiResponse<MemberDto>>> UpdateProfileApproval(
        Guid memberId, [FromBody] UpdateApprovalRequest request)
    {
        var tenantId = TenantUserContext.GetTenantId(User);
        var result = await _service.UpdateProfileApprovalAsync(tenantId, memberId, request.Status);
        if (result == null) return NotFound(ApiResponse<MemberDto>.Fail("Member not found."));
        return Ok(ApiResponse<MemberDto>.Ok(result, "Profile status updated."));
    }

    [HttpPut("{memberId:guid}/photo-approval")]
    public async Task<ActionResult<ApiResponse<MemberDto>>> UpdatePhotoApproval(
        Guid memberId, [FromBody] UpdateApprovalRequest request)
    {
        var tenantId = TenantUserContext.GetTenantId(User);
        var result = await _service.UpdatePhotoApprovalAsync(tenantId, memberId, request.Status);
        if (result == null) return NotFound(ApiResponse<MemberDto>.Fail("Member not found."));
        return Ok(ApiResponse<MemberDto>.Ok(result, "Photo status updated."));
    }
}
