namespace Matrimonial.AdminApi.Services;

public class StoredPhoto
{
    public string PhotoUrl { get; set; } = string.Empty;
    public byte[]? ImageData { get; set; }
    public string? ContentType { get; set; }
}
