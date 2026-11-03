using Veltrix.DTOs.ProductVariantDTO;

namespace Veltrix.Services.ProductVariant
{
    public interface IProductVariantService
    {
        Task<ProductVariantDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<ProductVariantDto>> GetByProductIdAsync(int productId);
        Task<ProductVariantDto> CreateAsync(CreateProductVariantDto dto);
        Task<ProductVariantDto?> UpdateAsync(int id, UpdateProductVariantDto dto);
        Task<bool> SetOptionsAsync(int variantId, SetVariantOptionsDto dto);
        Task<bool> SetDefaultAsync(int id);
        Task<bool> UpdateOptionsAsync(int variantId, SetVariantOptionsDto dto);
 
        Task<bool> ActivateAsync(int id);
        Task<bool> DeactivateAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
