using Matrimonial.AdminApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public MediaController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet("photos/{photoId:guid}")]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetPhoto(Guid photoId)
    {
        var photo = await _context.UserPhotos.AsNoTracking()
            .FirstOrDefaultAsync(p => p.PhotoId == photoId);

        if (photo?.ImageData is { Length: > 0 })
            return File(photo.ImageData, photo.ContentType ?? "image/jpeg");

        return NotFound();
    }

    [HttpGet("~/uploads/users/{userId:guid}/{fileName}")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public Task<IActionResult> GetLegacyUploadRoot(Guid userId, string fileName) =>
        GetLegacyUpload(userId, fileName);

    [HttpGet("uploads/users/{userId:guid}/{fileName}")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetLegacyUpload(Guid userId, string fileName)
    {
        var relativePath = $"/uploads/users/{userId}/{fileName}";
        var photo = await _context.UserPhotos.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId && p.PhotoUrl == relativePath);

        if (photo?.ImageData is { Length: > 0 })
            return File(photo.ImageData, photo.ContentType ?? "image/jpeg");

        var webRoot = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRoot))
            webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        var fullPath = Path.Combine(webRoot, "uploads", "users", userId.ToString(), fileName);
        if (!System.IO.File.Exists(fullPath))
            return NotFound();

        return PhysicalFile(fullPath, GetContentType(fileName));
    }

    private static string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            _ => "image/jpeg"
        };
    }
}
