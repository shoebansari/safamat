using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.UserPanel;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/user/profile")]
[Authorize(Roles = "User")]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _service;

    public UserProfileController(IUserProfileService service) => _service = service;

    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetMe()
    {
        var result = await _service.GetMyProfileAsync(UserContext.GetUserId(User));
        if (result == null) return NotFound(ApiResponse<UserProfileDto>.Fail("Profile not found."));
        return Ok(ApiResponse<UserProfileDto>.Ok(result));
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetPublic(Guid userId)
    {
        var result = await _service.GetPublicProfileAsync(UserContext.GetUserId(User), userId);
        if (result == null) return NotFound(ApiResponse<UserProfileDto>.Fail("Profile not found."));
        return Ok(ApiResponse<UserProfileDto>.Ok(result));
    }

    [HttpPut("basic")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> SaveBasic([FromBody] SaveProfileRequest request)
    {
        var result = await _service.SaveBasicProfileAsync(UserContext.GetUserId(User), request);
        return Ok(ApiResponse<UserProfileDto>.Ok(result!, "Profile saved."));
    }

    [HttpPut("education")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> SaveEducation([FromBody] SaveEducationRequest request)
    {
        var result = await _service.SaveEducationAsync(UserContext.GetUserId(User), request);
        return Ok(ApiResponse<UserProfileDto>.Ok(result!, "Education saved."));
    }

    [HttpPut("occupation")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> SaveOccupation([FromBody] SaveOccupationRequest request)
    {
        var result = await _service.SaveOccupationAsync(UserContext.GetUserId(User), request);
        return Ok(ApiResponse<UserProfileDto>.Ok(result!, "Occupation saved."));
    }

    [HttpPut("family")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> SaveFamily([FromBody] SaveFamilyRequest request)
    {
        var result = await _service.SaveFamilyAsync(UserContext.GetUserId(User), request);
        return Ok(ApiResponse<UserProfileDto>.Ok(result!, "Family details saved."));
    }

    [HttpPut("lifestyle")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> SaveLifestyle([FromBody] SaveLifestyleRequest request)
    {
        var result = await _service.SaveLifestyleAsync(UserContext.GetUserId(User), request);
        return Ok(ApiResponse<UserProfileDto>.Ok(result!, "Lifestyle saved."));
    }

    [HttpPut("location")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> SaveLocation([FromBody] SaveLocationRequest request)
    {
        var result = await _service.SaveLocationAsync(UserContext.GetUserId(User), request);
        return Ok(ApiResponse<UserProfileDto>.Ok(result!, "Location saved."));
    }

    [HttpGet("preferences")]
    public async Task<ActionResult<ApiResponse<UserPreferenceDto>>> GetPreferences()
    {
        var result = await _service.GetPreferencesAsync(UserContext.GetUserId(User));
        return Ok(ApiResponse<UserPreferenceDto>.Ok(result ?? new UserPreferenceDto()));
    }

    [HttpPut("preferences")]
    public async Task<ActionResult<ApiResponse<UserPreferenceDto>>> SavePreferences([FromBody] SavePreferenceRequest request)
    {
        var result = await _service.SavePreferencesAsync(UserContext.GetUserId(User), request);
        return Ok(ApiResponse<UserPreferenceDto>.Ok(result!, "Preferences saved."));
    }

    [HttpPost("photos/upload")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<ActionResult<ApiResponse<UserPhotoDto>>> UploadPhoto(
        IFormFile file, [FromForm] bool isPrimary = false)
    {
        try
        {
            var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(webRoot);
            var result = await _service.UploadPhotoAsync(
                UserContext.GetUserId(User), file, isPrimary, webRoot);
            return Ok(ApiResponse<UserPhotoDto>.Ok(result!, "Photo uploaded."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<UserPhotoDto>.Fail(ex.Message));
        }
    }

    [HttpPost("photos")]
    public async Task<ActionResult<ApiResponse<UserPhotoDto>>> AddPhoto([FromBody] AddPhotoRequest request)
    {
        var result = await _service.AddPhotoAsync(UserContext.GetUserId(User), request);
        return Ok(ApiResponse<UserPhotoDto>.Ok(result!, "Photo added."));
    }

    [HttpDelete("photos/{photoId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePhoto(Guid photoId)
    {
        var deleted = await _service.DeletePhotoAsync(UserContext.GetUserId(User), photoId);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Photo not found."));
        return Ok(ApiResponse<object>.Ok(new { }, "Photo deleted."));
    }
}
