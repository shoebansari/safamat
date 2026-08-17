using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.DTOs.EmailTemplates;
using Matrimonial.AdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class EmailTemplatesController : ControllerBase
{
    private readonly IEmailTemplateService _service;

    public EmailTemplatesController(IEmailTemplateService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EmailTemplateDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool? isActive = null)
    {
        var result = await _service.GetAllAsync(page, pageSize, isActive);
        return Ok(ApiResponse<PagedResult<EmailTemplateDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<EmailTemplateDto>>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse<EmailTemplateDto>.Fail("Template not found."));
        return Ok(ApiResponse<EmailTemplateDto>.Ok(result));
    }

    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<ApiResponse<EmailTemplateDto>>> GetByName(string name)
    {
        var result = await _service.GetByNameAsync(name);
        if (result == null) return NotFound(ApiResponse<EmailTemplateDto>.Fail("Template not found."));
        return Ok(ApiResponse<EmailTemplateDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<EmailTemplateDto>>> Create([FromBody] CreateEmailTemplateRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.TemplateId }, ApiResponse<EmailTemplateDto>.Ok(result, "Template created."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<EmailTemplateDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<EmailTemplateDto>>> Update(Guid id, [FromBody] UpdateEmailTemplateRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (result == null) return NotFound(ApiResponse<EmailTemplateDto>.Fail("Template not found."));
        return Ok(ApiResponse<EmailTemplateDto>.Ok(result, "Template updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Template not found."));
        return Ok(ApiResponse<object>.Ok(new { }, "Template deleted."));
    }
}
