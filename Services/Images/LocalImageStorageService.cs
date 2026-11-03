
namespace Veltrix.Services.Images
{
    public class LocalImageStorageService : IImageStorageService
    {
        private readonly IWebHostEnvironment _environment;

        private const long MaxFileSize = 5 * 1024 * 1024;

        private static readonly string[] AllowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        public LocalImageStorageService(
            IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveAsync(
            IFormFile image,
            string folderName)
        {
            if (image == null || image.Length == 0)
                throw new InvalidOperationException(
                    "Please select an image.");

            if (image.Length > MaxFileSize)
                throw new InvalidOperationException(
                    "Image size cannot exceed 5 MB.");

            var extension = Path.GetExtension(image.FileName)
                .ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException(
                    "Only JPG, JPEG, PNG and WEBP images are allowed.");

            var uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                folderName);

            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(
                uploadsFolder,
                fileName);

            await using (var stream = new FileStream(
                filePath,
                FileMode.CreateNew))
            {
                await image.CopyToAsync(stream);
            }

            return $"/uploads/{folderName}/{fileName}";
        }

        public Task DeleteAsync(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return Task.CompletedTask;

            var uploadsRoot = Path.GetFullPath(
                Path.Combine(_environment.WebRootPath, "uploads"));

            var relativePath = imageUrl
                .TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar);

            if (!relativePath.StartsWith(
                $"uploads{Path.DirectorySeparatorChar}",
                StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            var filePath = Path.GetFullPath(
                Path.Combine(_environment.WebRootPath, relativePath));

            if (!filePath.StartsWith(
                uploadsRoot + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            if (File.Exists(filePath))
                File.Delete(filePath);

            return Task.CompletedTask;
        }
    }
}
