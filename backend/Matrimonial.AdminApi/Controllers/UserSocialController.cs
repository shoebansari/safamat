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
public class UserSocialController : ControllerBase
{
    private readonly IUserSocialService _social;
    private readonly IUserMessageService _messages;
    private readonly IUserNotificationService _notifications;

    public UserSocialController(
        IUserSocialService social,
        IUserMessageService messages,
        IUserNotificationService notifications)
    {
        _social = social;
        _messages = messages;
        _notifications = notifications;
    }

    [HttpGet("discover")]
    public async Task<ActionResult<ApiResponse<List<DiscoverProfileDto>>>> Discover([FromQuery] DiscoverFilterRequest filters)
    {
        var userId = UserContext.GetUserId(User);
        var tenantId = UserContext.GetTenantId(User);
        var result = await _social.DiscoverAsync(userId, tenantId, filters);
        return Ok(ApiResponse<List<DiscoverProfileDto>>.Ok(result));
    }

    [HttpGet("discover/filter-options")]
    public async Task<ActionResult<ApiResponse<DiscoverFilterOptionsDto>>> DiscoverFilterOptions()
    {
        var userId = UserContext.GetUserId(User);
        var tenantId = UserContext.GetTenantId(User);
        var result = await _social.GetDiscoverFilterOptionsAsync(userId, tenantId);
        return Ok(ApiResponse<DiscoverFilterOptionsDto>.Ok(result));
    }

    [HttpGet("interests")]
    public async Task<ActionResult<ApiResponse<List<InterestRequestDto>>>> Interests([FromQuery] string type = "received")
    {
        var result = await _social.GetInterestsAsync(UserContext.GetUserId(User), type);
        return Ok(ApiResponse<List<InterestRequestDto>>.Ok(result));
    }

    [HttpPost("interests/{receiverId:guid}")]
    public async Task<ActionResult<ApiResponse<InterestRequestDto>>> SendInterest(Guid receiverId)
    {
        try
        {
            var result = await _social.SendInterestAsync(UserContext.GetUserId(User), receiverId);
            if (result == null) return BadRequest(ApiResponse<InterestRequestDto>.Fail("Cannot send interest."));
            return Ok(ApiResponse<InterestRequestDto>.Ok(result, "Interest sent."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<InterestRequestDto>.Fail(ex.Message));
        }
    }

    [HttpPut("interests/{interestId:guid}")]
    public async Task<ActionResult<ApiResponse<InterestRequestDto>>> RespondInterest(
        Guid interestId, [FromBody] UpdateApprovalRequest request)
    {
        var result = await _social.RespondInterestAsync(UserContext.GetUserId(User), interestId, request.Status);
        if (result == null) return NotFound(ApiResponse<InterestRequestDto>.Fail("Interest not found."));
        return Ok(ApiResponse<InterestRequestDto>.Ok(result, "Interest updated."));
    }

    [HttpGet("matches")]
    public async Task<ActionResult<ApiResponse<List<MatchDto>>>> Matches()
    {
        var result = await _social.GetMatchesAsync(UserContext.GetUserId(User));
        return Ok(ApiResponse<List<MatchDto>>.Ok(result));
    }

    [HttpGet("favorites")]
    public async Task<ActionResult<ApiResponse<List<DiscoverProfileDto>>>> Favorites()
    {
        var result = await _social.GetFavoritesAsync(UserContext.GetUserId(User));
        return Ok(ApiResponse<List<DiscoverProfileDto>>.Ok(result));
    }

    [HttpPost("favorites/{userId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> ToggleFavorite(Guid userId)
    {
        var added = await _social.ToggleFavoriteAsync(UserContext.GetUserId(User), userId);
        return Ok(ApiResponse<object>.Ok(new { added }, added ? "Added to favorites." : "Removed from favorites."));
    }

    [HttpPost("block/{userId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Block(Guid userId)
    {
        await _social.BlockUserAsync(UserContext.GetUserId(User), userId);
        return Ok(ApiResponse<object>.Ok(new { }, "User blocked."));
    }

    [HttpPost("report")]
    public async Task<ActionResult<ApiResponse<object>>> Report([FromBody] ReportUserRequest request)
    {
        await _social.ReportUserAsync(UserContext.GetUserId(User), request);
        return Ok(ApiResponse<object>.Ok(new { }, "Report submitted."));
    }

    [HttpGet("conversations")]
    public async Task<ActionResult<ApiResponse<List<ConversationDto>>>> Conversations()
    {
        var result = await _messages.GetConversationsAsync(UserContext.GetUserId(User));
        return Ok(ApiResponse<List<ConversationDto>>.Ok(result));
    }

    [HttpGet("messages/{otherUserId:guid}")]
    public async Task<ActionResult<ApiResponse<List<MessageDto>>>> Messages(Guid otherUserId)
    {
        await _messages.MarkReadAsync(UserContext.GetUserId(User), otherUserId);
        var result = await _messages.GetMessagesAsync(UserContext.GetUserId(User), otherUserId);
        return Ok(ApiResponse<List<MessageDto>>.Ok(result));
    }

    [HttpPost("messages")]
    public async Task<ActionResult<ApiResponse<MessageDto>>> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            var result = await _messages.SendMessageAsync(UserContext.GetUserId(User), request);
            if (result == null) return BadRequest(ApiResponse<MessageDto>.Fail("Failed to send."));
            return Ok(ApiResponse<MessageDto>.Ok(result, "Message sent."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<MessageDto>.Fail(ex.Message));
        }
    }

    [HttpGet("notifications")]
    public async Task<ActionResult<ApiResponse<List<NotificationDto>>>> Notifications()
    {
        var result = await _notifications.GetAllAsync(UserContext.GetUserId(User));
        return Ok(ApiResponse<List<NotificationDto>>.Ok(result));
    }

    [HttpGet("notifications/unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> UnreadCount()
    {
        var count = await _notifications.GetUnreadCountAsync(UserContext.GetUserId(User));
        return Ok(ApiResponse<int>.Ok(count));
    }

    [HttpPut("notifications/{id:guid}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkRead(Guid id)
    {
        await _notifications.MarkReadAsync(UserContext.GetUserId(User), id);
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    [HttpPut("notifications/read-all")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllRead()
    {
        await _notifications.MarkAllReadAsync(UserContext.GetUserId(User));
        return Ok(ApiResponse<object>.Ok(new { }));
    }
}
