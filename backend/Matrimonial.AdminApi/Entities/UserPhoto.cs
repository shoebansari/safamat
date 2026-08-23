namespace Matrimonial.AdminApi.Entities;

public class UserPhoto
{
    public Guid PhotoId { get; set; }
    public Guid UserId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public byte[]? ImageData { get; set; }
    public string? ContentType { get; set; }
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsApproved { get; set; }
    public DateTime UploadedOn { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
