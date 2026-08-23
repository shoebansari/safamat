namespace Matrimonial.AdminApi.Services;

public interface IPhotoStorageService
{
    Task<StoredPhoto> StoreUserPhotoAsync(Guid photoId, Guid userId, IFormFile file);
}
