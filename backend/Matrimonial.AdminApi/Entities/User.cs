namespace Matrimonial.AdminApi.Entities;

public class User
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string UserCode { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Password { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public UserProfile? Profile { get; set; }
    public ICollection<UserPhoto> Photos { get; set; } = new List<UserPhoto>();
    public ICollection<UserEducation> Educations { get; set; } = new List<UserEducation>();
    public UserOccupation? Occupation { get; set; }
    public UserFamilyDetail? Family { get; set; }
    public UserLifestyle? Lifestyle { get; set; }
    public UserLocation? Location { get; set; }
    public UserPreference? Preference { get; set; }
    public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
}
