namespace Matrimonial.AdminApi.Entities;

public class BlockedUser
{
    public Guid BlockedId { get; set; }
    public Guid UserId { get; set; }
    public Guid BlockedUserId { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public User Blocked { get; set; } = null!;
}
