using TerraLink.Api.Models;

public interface IFileStorageService
{
    Task<string> SaveAsync(
        IFormFile file,
        StorageFolder folder,
        CancellationToken cancellationToken = default
    );

    Task DeleteAsync(
        string fileurl,
        CancellationToken cancellationToken = default
    );
}