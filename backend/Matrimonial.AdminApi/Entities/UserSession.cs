namespace Matrimonial.AdminApi.Entities;

public class UserSession
{
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public string? RefreshToken { get; set; }
    public string? DeviceName { get; set; }
    public string? Browser { get; set; }
    public string? IPAddress { get; set; }
    public DateTime LoginTime { get; set; } = DateTime.UtcNow;
    public DateTime? LogoutTime { get; set; }

    public User User { get; set; } = null!;
}
