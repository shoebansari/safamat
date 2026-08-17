namespace Matrimonial.AdminApi.Entities;

public class AdminUser
{
    public Guid AdminId { get; set; }
    public string AdminUserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }

    public ICollection<Tenant> CreatedTenants { get; set; } = new List<Tenant>();
}
