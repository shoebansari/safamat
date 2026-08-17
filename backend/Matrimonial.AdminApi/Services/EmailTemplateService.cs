using Matrimonial.AdminApi.Common;
using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.EmailTemplates;
using Matrimonial.AdminApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface IEmailTemplateService
{
    Task<PagedResult<EmailTemplateDto>> GetAllAsync(int page, int pageSize, bool? isActive);
    Task<EmailTemplateDto?> GetByIdAsync(Guid id);
    Task<EmailTemplateDto?> GetByNameAsync(string name);
    Task<EmailTemplateDto> CreateAsync(CreateEmailTemplateRequest request);
    Task<EmailTemplateDto?> UpdateAsync(Guid id, UpdateEmailTemplateRequest request);
    Task<bool> DeleteAsync(Guid id);
}

public class EmailTemplateService : IEmailTemplateService
{
    private readonly ApplicationDbContext _context;

    public EmailTemplateService(ApplicationDbContext context) => _context = context;

    public async Task<PagedResult<EmailTemplateDto>> GetAllAsync(int page, int pageSize, bool? isActive)
    {
        var query = _context.EmailTemplates.AsQueryable();

        if (isActive.HasValue)
            query = query.Where(t => t.IsActive == isActive.Value);

        var total = await query.CountAsync();
        var templates = await query
            .OrderBy(t => t.TemplateName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = templates.Select(MapToDto).ToList();

        return new PagedResult<EmailTemplateDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<EmailTemplateDto?> GetByIdAsync(Guid id)
    {
        var template = await _context.EmailTemplates.FindAsync(id);
        return template == null ? null : MapToDto(template);
    }

    public async Task<EmailTemplateDto?> GetByNameAsync(string name)
    {
        var template = await _context.EmailTemplates.FirstOrDefaultAsync(t => t.TemplateName == name);
        return template == null ? null : MapToDto(template);
    }

    public async Task<EmailTemplateDto> CreateAsync(CreateEmailTemplateRequest request)
    {
        if (await _context.EmailTemplates.AnyAsync(t => t.TemplateName == request.TemplateName))
            throw new InvalidOperationException("Template name already exists.");

        var template = new EmailTemplate
        {
            TemplateId = Guid.NewGuid(),
            TemplateName = request.TemplateName,
            Subject = request.Subject,
            Body = request.Body,
            IsActive = request.IsActive
        };

        _context.EmailTemplates.Add(template);
        await _context.SaveChangesAsync();
        return MapToDto(template);
    }

    public async Task<EmailTemplateDto?> UpdateAsync(Guid id, UpdateEmailTemplateRequest request)
    {
        var template = await _context.EmailTemplates.FindAsync(id);
        if (template == null) return null;

        if (request.Subject != null) template.Subject = request.Subject;
        if (request.Body != null) template.Body = request.Body;
        if (request.IsActive.HasValue) template.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync();
        return MapToDto(template);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var template = await _context.EmailTemplates.FindAsync(id);
        if (template == null) return false;

        template.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static EmailTemplateDto MapToDto(EmailTemplate t) => new()
    {
        TemplateId = t.TemplateId,
        TemplateName = t.TemplateName,
        Subject = t.Subject,
        Body = t.Body,
        IsActive = t.IsActive
    };
}
