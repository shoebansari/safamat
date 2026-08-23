using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.TenantPanel;
using Matrimonial.AdminApi.DTOs.UserPanel;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/tenant/users")]
[Authorize(Roles = "Tenant")]
public class TenantPanelUsersController : ControllerBase
{
    private readonly ITenantUserService _service;

    public TenantPanelUsersController(ITenantUserService service) => _service = service;

    [HttpGet("detail/{userId:guid}")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetDetail(Guid userId)
    {
        var tenantId = TenantUserContext.GetTenantId(User);
        var result = await _service.GetUserProfileDetailAsync(tenantId, userId);
        if (result == null) return NotFound(ApiResponse<UserProfileDto>.Fail("User not found."));
        return Ok(ApiResponse<UserProfileDto>.Ok(result));
    }

    [HttpGet("pending-approvals")]
    public async Task<ActionResult<ApiResponse<List<TenantUserDto>>>> GetPendingApprovals()
    {
        var tenantId = TenantUserContext.GetTenantId(User);
        var result = await _service.GetPendingApprovalsAsync(tenantId);
        return Ok(ApiResponse<List<TenantUserDto>>.Ok(result));
    }

    [HttpGet("{userCode}")]
    public async Task<ActionResult<ApiResponse<TenantUserDto>>> GetByUserCode(string userCode)
    {
        var tenantId = TenantUserContext.GetTenantId(User);
        var result = await _service.GetByUserCodeAsync(tenantId, userCode);
        if (result == null) return NotFound(ApiResponse<TenantUserDto>.Fail("User not found."));
        return Ok(ApiResponse<TenantUserDto>.Ok(result));
    }

    [HttpPut("{userCode}/plan")]
    public async Task<ActionResult<ApiResponse<TenantUserDto>>> UpdatePlan(
        string userCode, [FromBody] UpdateMemberPlanAssignmentRequest request)
    {
        try
        {
            var tenantId = TenantUserContext.GetTenantId(User);
            var result = await _service.UpdatePlanAssignmentAsync(tenantId, userCode, request);
            if (result == null) return NotFound(ApiResponse<TenantUserDto>.Fail("User not found."));
            return Ok(ApiResponse<TenantUserDto>.Ok(result, "Plan updated."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<TenantUserDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{userId:guid}/profile-approval")]
    public async Task<ActionResult<ApiResponse<TenantUserDto>>> UpdateProfileApproval(
        Guid userId, [FromBody] UpdateApprovalRequest request)
    {
        var tenantId = TenantUserContext.GetTenantId(User);
        var result = await _service.UpdateProfileApprovalAsync(tenantId, userId, request.Status);
        if (result == null) return NotFound(ApiResponse<TenantUserDto>.Fail("User not found."));
        return Ok(ApiResponse<TenantUserDto>.Ok(result, "Profile status updated."));
    }

    [HttpPut("photos/{photoId:guid}/approval")]
    public async Task<ActionResult<ApiResponse<TenantUserDto>>> UpdatePhotoApproval(
        Guid photoId, [FromBody] UpdateApprovalRequest request)
    {
        var tenantId = TenantUserContext.GetTenantId(User);
        var result = await _service.UpdatePhotoApprovalAsync(tenantId, photoId, request.Status);
        if (result == null) return NotFound(ApiResponse<TenantUserDto>.Fail("Photo not found."));
        return Ok(ApiResponse<TenantUserDto>.Ok(result, "Photo status updated."));
    }
}
