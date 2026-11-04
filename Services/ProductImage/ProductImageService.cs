using Veltrix.DTOs.ProductImageDTO;
using Veltrix.Repositories.ProductImage;
using Veltrix.Services.FileStorage;
using Veltrix.UnitOfWork;

namespace Veltrix.Services.ProductImage
{
    public class ProductImageService : IProductImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private const string UploadFolder = "uploads/products";
        public ProductImageService(IUnitOfWork unitOfWork , IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        private static ProductImageDto MapToDto(Models.ProductImage image)
        {
            return new ProductImageDto
            {
                Id = image.Id,
                ProductId = image.ProductId,
                ImageUrl = image.ImageUrl,
                AltText = image.AltText,
                IsPrimary = image.IsPrimary,
                DisplayOrder = image.DisplayOrder,
                CreatedAt = image.CreatedAt
            };
        }
        public async Task<ProductImageDto> CreateAsync(int productId, CreateProductImageDto dto)
        {
            if (productId <= 0)
                throw new ArgumentException("Invalid product ID.");

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Image == null || dto.Image.Length == 0)
                throw new ArgumentException("An image is required.");

            var product = await _unitOfWork.Product.GetByIdAsync(productId);

            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            var existingImages = await _unitOfWork.ProductImages
                .GetByProductIdAsync(productId);

            bool shouldBePrimary =
                dto.IsPrimary || !existingImages.Any();

            string imagePath = await _fileStorageService.SaveImageAsync(
                dto.Image,
                UploadFolder);

            try
            {
                await using var transaction =
                    await _unitOfWork.BeginTransactionAsync();

                try
                {
                    if (shouldBePrimary)
                    {
                        foreach (var existingImage in existingImages)
                        {
                            if (existingImage.IsPrimary)
                            {
                                existingImage.IsPrimary = false;
                                _unitOfWork.ProductImages.Update(existingImage);
                            }
                        }

                        await _unitOfWork.SaveChangesAsync();
                    }

                    var image = new Models.ProductImage
                    {
                        ProductId = productId,
                        ImageUrl = imagePath,
                        AltText = dto.AltText?.Trim(),
                        IsPrimary = shouldBePrimary,
                        DisplayOrder = dto.DisplayOrder,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.ProductImages.AddAsync(image);

                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return MapToDto(image);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch
            {
                await _fileStorageService.DeleteFileAsync(imagePath);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                return false;

            var image = await _unitOfWork.ProductImages.GetByIdAsync(id);

            if (image == null)
                return false;

            var imagePath = image.ImageUrl;

            var images = await _unitOfWork.ProductImages
                .GetByProductIdAsync(image.ProductId);

            var nextPrimary = image.IsPrimary
                ? images.FirstOrDefault(x => x.Id != id)
                : null;

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Remove the primary flag first to avoid unique index conflicts.
                if (image.IsPrimary)
                {
                    image.IsPrimary = false;

                    _unitOfWork.ProductImages.Update(image);

                    await _unitOfWork.SaveChangesAsync();
                }

                _unitOfWork.ProductImages.Delete(image);

                if (nextPrimary != null)
                {
                    nextPrimary.IsPrimary = true;

                    _unitOfWork.ProductImages.Update(nextPrimary);
                }

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            // Delete the physical file after the database transaction succeeds.
            await _fileStorageService.DeleteFileAsync(imagePath);

            return true;
        }

        public async Task<ProductImageDto?> GetByIdAsync(int id)
        {
            var image = await _unitOfWork.ProductImages.GetByIdAsync(id);

            if (image == null)
                return null;

            return MapToDto(image);
        }

        public async Task<IReadOnlyList<ProductImageDto>> GetByProductIdAsync(int productId)
        {
            if(productId <= 0)
                throw new ArgumentException("Invalid product ID.");

            var images = await _unitOfWork.ProductImages.GetByProductIdAsync(productId);

            return images.Select(MapToDto).ToList();

        }

        public async Task<bool> SetPrimaryAsync(int id)
        {
            if (id <= 0)
                return false;

            var image = await _unitOfWork.ProductImages.GetByIdAsync(id);

            if (image == null)
                return false;

            if (image.IsPrimary)
                return true;

            var images = await _unitOfWork.ProductImages
                .GetByProductIdAsync(image.ProductId);

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync();

            try
            {
                // First remove the current primary image.
                foreach (var item in images)
                {
                    if (item.IsPrimary)
                    {
                        item.IsPrimary = false;
                        _unitOfWork.ProductImages.Update(item);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // Then assign the new primary image.
                image.IsPrimary = true;

                _unitOfWork.ProductImages.Update(image);

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductImageDto dto)
        {
            if (id <= 0)
                return false;

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var image = await _unitOfWork.ProductImages.GetByIdAsync(id);

            if (image == null)
                return false;

            string? newImagePath = null;
            string oldImagePath = image.ImageUrl;

            if (dto.Image != null && dto.Image.Length > 0)
            {
                newImagePath = await _fileStorageService.SaveImageAsync(
                    dto.Image,
                    UploadFolder);
            }

            try
            {
                image.AltText = dto.AltText?.Trim();
                image.DisplayOrder = dto.DisplayOrder;

                if (newImagePath != null)
                    image.ImageUrl = newImagePath;

                _unitOfWork.ProductImages.Update(image);

                await _unitOfWork.SaveChangesAsync();

                // Delete the old file only after the database update succeeds.
                if (newImagePath != null)
                {
                    await _fileStorageService.DeleteFileAsync(oldImagePath);
                }

                return true;
            }
            catch
            {
                if (newImagePath != null)
                    await _fileStorageService.DeleteFileAsync(newImagePath);

                throw;
            }
        }
    }
}
