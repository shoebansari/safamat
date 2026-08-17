namespace Matrimonial.AdminApi.DTOs.EmailTemplates;

public class EmailTemplateDto
{
    public Guid TemplateId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateEmailTemplateRequest
{
    public string TemplateName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class UpdateEmailTemplateRequest
{
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public bool? IsActive { get; set; }
}
