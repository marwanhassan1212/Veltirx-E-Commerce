namespace Veltrix.Services.FileStorage
{
    public interface IFileStorageService
    {
        Task<string> SaveImageAsync(IFormFile image, string folder);
        Task DeleteFileAsync(string filePath);
    }
}