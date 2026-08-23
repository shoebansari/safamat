namespace Matrimonial.AdminApi.DTOs.Tenants;

public class TenantDto
{
    public Guid TenantId { get; set; }
    public string TenantCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? LogoUrl { get; set; }
    public string? DatabaseName { get; set; }
    public string? DatabaseServer { get; set; }
    public bool IsActive { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

public class CreateTenantRequest
{
    public string TenantCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? LogoUrl { get; set; }
    public string? DatabaseName { get; set; }
    public string? DatabaseServer { get; set; }
    public string? ConnectionString { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateTenantRequest
{
    public string? CompanyName { get; set; }
    public string? OwnerName { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? LogoUrl { get; set; }
    public string? DatabaseName { get; set; }
    public string? DatabaseServer { get; set; }
    public string? ConnectionString { get; set; }
    public bool? IsActive { get; set; }
}
