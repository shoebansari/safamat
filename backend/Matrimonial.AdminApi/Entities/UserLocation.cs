namespace Matrimonial.AdminApi.Entities;

public class UserLocation
{
    public Guid LocationId { get; set; }
    public Guid UserId { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? Pincode { get; set; }

    public User User { get; set; } = null!;
}
