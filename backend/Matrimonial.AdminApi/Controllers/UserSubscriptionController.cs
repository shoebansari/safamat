using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.TenantPanel;
using Matrimonial.AdminApi.DTOs.UserPanel;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/user")]
[Authorize(Roles = "User")]
public class UserSubscriptionController : ControllerBase
{
    private readonly IUserSubscriptionService _subscriptions;

    public UserSubscriptionController(IUserSubscriptionService subscriptions) =>
        _subscriptions = subscriptions;

    [HttpGet("plans")]
    public async Task<ActionResult<ApiResponse<List<MemberPlanDto>>>> GetPlans()
    {
        var tenantId = UserContext.GetTenantId(User);
        var result = await _subscriptions.GetPlansForUserAsync(tenantId);
        return Ok(ApiResponse<List<MemberPlanDto>>.Ok(result));
    }

    [HttpGet("subscription")]
    public async Task<ActionResult<ApiResponse<UserSubscriptionDto?>>> GetSubscription()
    {
        var result = await _subscriptions.GetMySubscriptionAsync(UserContext.GetUserId(User));
        return Ok(ApiResponse<UserSubscriptionDto?>.Ok(result));
    }

    [HttpPut("subscription")]
    public async Task<ActionResult<ApiResponse<UserSubscriptionDto>>> ChangePlan([FromBody] ChangePlanRequest request)
    {
        try
        {
            var result = await _subscriptions.RequestPlanChangeAsync(
                UserContext.GetUserId(User),
                UserContext.GetTenantId(User),
                request.MemberPlanId);
            return Ok(ApiResponse<UserSubscriptionDto>.Ok(result, "Plan change requested. Waiting for tenant approval."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<UserSubscriptionDto>.Fail(ex.Message));
        }
    }
}
