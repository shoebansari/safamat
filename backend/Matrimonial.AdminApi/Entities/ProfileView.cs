namespace Matrimonial.AdminApi.Entities;

public class ProfileView
{
    public Guid ViewId { get; set; }
    public Guid ViewerUserId { get; set; }
    public Guid ViewedUserId { get; set; }
    public DateTime ViewedOn { get; set; } = DateTime.UtcNow;

    public User Viewer { get; set; } = null!;
    public User Viewed { get; set; } = null!;
}
