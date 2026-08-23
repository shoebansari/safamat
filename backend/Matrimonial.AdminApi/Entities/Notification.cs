namespace Matrimonial.AdminApi.Entities;

public class Notification
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
    public Guid? RelatedUserId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
