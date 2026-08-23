namespace Matrimonial.AdminApi.Entities;

public class Favorite
{
    public Guid FavoriteId { get; set; }
    public Guid UserId { get; set; }
    public Guid FavoriteUserId { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public User FavoriteUser { get; set; } = null!;
}
