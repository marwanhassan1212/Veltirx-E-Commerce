namespace Veltrix.Services.Images
{
    public interface IImageStorageService
    {
        Task<string> SaveAsync(IFormFile image, string folderName);
        Task DeleteAsync(string? imageUrl);
    }
}
