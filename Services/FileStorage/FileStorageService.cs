
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Veltrix.Services.FileStorage
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveImageAsync(IFormFile image, string folder)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("No image was provided.");

            if (image.Length > MaxFileSize)
                throw new ArgumentException("Image size cannot exceed 5 MB.");

            if (string.IsNullOrWhiteSpace(folder))
                throw new ArgumentException("Upload folder is required.");

            var extension = Path.GetExtension(image.FileName);

            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException("Unsupported image format.");

            // Generate a unique file name
            var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

            // Get wwwroot path
            var webRootPath = _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new InvalidOperationException("Web root path is not configured.");

            // Build and validate the target directory
            var rootPath = Path.GetFullPath(webRootPath);
            var folderPath = Path.GetFullPath(
                Path.Combine(rootPath, folder));

            if (!folderPath.StartsWith(
                rootPath + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid upload folder.");
            }

            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            // Save the image
            await using (var stream = new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                81920,
                useAsync: true))
            {
                await image.CopyToAsync(stream);
            }

            // Return a relative URL
            var relativePath = Path.Combine(folder, fileName)
                .Replace("\\", "/");

            return "/" + relativePath.TrimStart('/');
        }

        public Task DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return Task.CompletedTask;

            var webRootPath = _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new InvalidOperationException("Web root path is not configured.");

            var rootPath = Path.GetFullPath(webRootPath);

            // Remove the leading slash from the stored URL
            var relativePath = filePath.TrimStart('/', '\\');

            var fullPath = Path.GetFullPath(
                Path.Combine(rootPath, relativePath));

            // Ensure the file belongs to wwwroot
            if (!fullPath.StartsWith(
                rootPath + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid file path.");
            }

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }
    }
}
