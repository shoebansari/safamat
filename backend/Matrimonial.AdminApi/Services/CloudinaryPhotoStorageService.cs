using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Matrimonial.AdminApi.Configurations;
using Microsoft.Extensions.Options;

namespace Matrimonial.AdminApi.Services;

public class CloudinaryPhotoStorageService : IPhotoStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryPhotoStorageService(IOptions<CloudinarySettings> settings)
    {
        var config = settings.Value;
        var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<StoredPhoto> StoreUserPhotoAsync(Guid photoId, Guid userId, IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = $"matrimonial/users/{userId}",
            PublicId = photoId.ToString("N"),
            UseFilename = false,
            UniqueFilename = false,
            Overwrite = true
        };

        var result = await _cloudinary.UploadAsync(uploadParams);
        if (result.Error != null)
            throw new InvalidOperationException($"Photo upload failed: {result.Error.Message}");

        return new StoredPhoto
        {
            PhotoUrl = result.SecureUrl?.ToString()
                ?? throw new InvalidOperationException("Photo upload failed: no URL returned.")
        };
    }
}
