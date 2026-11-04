using Veltrix.DTOs.ProductVariantImage;

namespace Veltrix.Services.ProductVariantImage
{
    public interface IProductVariantImageService
    {
        Task<ProductVariantImageDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<ProductVariantImageDto>> GetByVariantIdAsync(int variantId);
        Task<ProductVariantImageDto?> CreateAsync(int variantId, CreateProductVariantImageDto dto);
        Task<bool> UpdateAsync(int id, UpdateProductVariantImageDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> SetPrimaryAsync(int id);
    }
}