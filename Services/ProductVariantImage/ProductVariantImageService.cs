using Veltrix.DTOs.ProductVariantImage;
using Veltrix.Models;
using Veltrix.Services.FileStorage;
using Veltrix.UnitOfWork;

namespace Veltrix.Services.ProductVariantImage
{
    public class ProductVariantImageService : IProductVariantImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;

        private const string UploadFolder = "uploads/product-variants";

        public ProductVariantImageService(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
        }

        private static ProductVariantImageDto MapToDto(
            Models.ProductVariantImage image)
        {
            return new ProductVariantImageDto
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl,
                AltText = image.AltText,
                IsPrimary = image.IsPrimary,
                DisplayOrder = image.DisplayOrder,
                CreatedAt = image.CreatedAt,
                ProductVariantId = image.ProductVariantId
            };
        }

        public async Task<ProductVariantImageDto?> GetByIdAsync(int id)
        {
            var image = await _unitOfWork.ProductVariantImages.GetByIdAsync(id);

            return image == null ? null : MapToDto(image);
        }

        public async Task<IReadOnlyList<ProductVariantImageDto>> GetByVariantIdAsync(
            int variantId)
        {
            var variant = await _unitOfWork.productVariant.GetByIdAsync(variantId);

            if (variant == null)
                return Array.Empty<ProductVariantImageDto>();

            var images = await _unitOfWork.ProductVariantImages
                .GetByVariantIdAsync(variantId);

            return images.Select(MapToDto).ToList();
        }

        public async Task<ProductVariantImageDto?> CreateAsync(
            int variantId,
            CreateProductVariantImageDto dto)
        {
            var variant = await _unitOfWork.productVariant.GetByIdAsync(variantId);

            if (variant == null)
                return null;

            var imageUrl = await _fileStorage.SaveImageAsync(
                dto.Image, UploadFolder);

            try
            {
                await using var transaction =
                    await _unitOfWork.BeginTransactionAsync();

                var existingImages = await _unitOfWork.ProductVariantImages
                    .GetByVariantIdAsync(variantId);

                if (dto.IsPrimary)
                {
                    foreach (var existingImage in existingImages)
                    {
                        existingImage.IsPrimary = false;
                        _unitOfWork.ProductVariantImages.Update(existingImage);
                    }
                }

                var image = new Models.ProductVariantImage
                {
                    ImageUrl = imageUrl,
                    AltText = dto.AltText,
                    IsPrimary = dto.IsPrimary,
                    DisplayOrder = dto.DisplayOrder,
                    CreatedAt = DateTime.UtcNow,
                    ProductVariantId = variantId
                };

                await _unitOfWork.ProductVariantImages.AddAsync(image);

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return MapToDto(image);
            }
            catch
            {
                await _fileStorage.DeleteFileAsync(imageUrl);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateProductVariantImageDto dto)
        {
            var image = await _unitOfWork.ProductVariantImages.GetByIdAsync(id);

            if (image == null)
                return false;

            string? newImageUrl = null;
            var oldImageUrl = image.ImageUrl;

            if (dto.Image != null)
            {
                newImageUrl = await _fileStorage.SaveImageAsync(
                    dto.Image, UploadFolder);
            }

            try
            {
                image.AltText = dto.AltText;
                image.DisplayOrder = dto.DisplayOrder;

                if (newImageUrl != null)
                    image.ImageUrl = newImageUrl;

                _unitOfWork.ProductVariantImages.Update(image);

                await _unitOfWork.SaveChangesAsync();

                if (newImageUrl != null)
                    await _fileStorage.DeleteFileAsync(oldImageUrl);

                return true;
            }
            catch
            {
                if (newImageUrl != null)
                    await _fileStorage.DeleteFileAsync(newImageUrl);

                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var image = await _unitOfWork.ProductVariantImages.GetByIdAsync(id);

            if (image == null)
                return false;

            var imageUrl = image.ImageUrl;

            _unitOfWork.ProductVariantImages.Delete(image);

            await _unitOfWork.SaveChangesAsync();

            await _fileStorage.DeleteFileAsync(imageUrl);

            return true;
        }

        public async Task<bool> SetPrimaryAsync(int id)
        {
            var image = await _unitOfWork.ProductVariantImages.GetByIdAsync(id);

            if (image == null)
                return false;

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync();

            var images = await _unitOfWork.ProductVariantImages
                .GetByVariantIdAsync(image.ProductVariantId);

            foreach (var variantImage in images)
            {
                variantImage.IsPrimary = variantImage.Id == id;

                _unitOfWork.ProductVariantImages.Update(variantImage);
            }

            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
    }
}