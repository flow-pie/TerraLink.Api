using TerraLink.Api.Models;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;

    public LocalFileStorageService(
        IWebHostEnvironment environment
    )
    {
        _environment = environment;
    }
    public async Task<string> SaveAsync(
        IFormFile file,
        StorageFolder folder,
        CancellationToken cancellationToken = default)
    {
        var folderName = folder switch
        {
            StorageFolder.Kyc => "kyc",
            StorageFolder.ProfilePhotos => "profile-photos",
            StorageFolder.Reports => "reports",
            _ => throw new ArgumentOutOfRangeException(nameof(folder))
        };

        var uploadsRoot = Path.Combine(
            _environment.WebRootPath
                ?? Path.Combine(_environment.ContentRootPath, "wwwroot"),
            "uploads",
            folderName);



        Directory.CreateDirectory(uploadsRoot);

        var fileName =
            $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        var fullPath = Path.Combine(
            uploadsRoot,
            fileName);

        await using var stream =
            File.Create(fullPath);

        await file.CopyToAsync(stream, cancellationToken);

        return $"/uploads/{folder}/{fileName}";
    }

    public Task DeleteAsync(
        string fileUrl,
        CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(
            _environment.WebRootPath,
            fileUrl.TrimStart('/'));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

}