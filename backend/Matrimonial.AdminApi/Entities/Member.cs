namespace Matrimonial.AdminApi.Entities;

public class Member
{
    public Guid MemberId { get; set; }
    public Guid TenantId { get; set; }
    public string UserCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string ProfileStatus { get; set; } = "Pending";
    public string PhotoStatus { get; set; } = "Pending";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public Tenant Tenant { get; set; } = null!;
    public ICollection<MemberSubscription> Subscriptions { get; set; } = new List<MemberSubscription>();
}
