namespace Matrimonial.AdminApi.Services;

public class DatabasePhotoStorageService : IPhotoStorageService
{
    public async Task<StoredPhoto> StoreUserPhotoAsync(Guid photoId, Guid userId, IFormFile file)
    {
        await using var memory = new MemoryStream();
        await file.CopyToAsync(memory);

        return new StoredPhoto
        {
            PhotoUrl = $"/api/media/photos/{photoId}",
            ImageData = memory.ToArray(),
            ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "image/jpeg" : file.ContentType
        };
    }
}
