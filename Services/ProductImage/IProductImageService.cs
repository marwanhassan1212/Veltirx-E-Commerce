using Veltrix.DTOs.ProductImageDTO;

namespace Veltrix.Services.ProductImage
{
    public interface IProductImageService
    {
        Task<IReadOnlyList<ProductImageDto>> GetByProductIdAsync(int productId);
        Task<ProductImageDto?> GetByIdAsync(int id);
        Task<ProductImageDto> CreateAsync(int productId, CreateProductImageDto dto);
        Task<bool> UpdateAsync(int id, UpdateProductImageDto dto);
        Task<bool> SetPrimaryAsync(int id);

        Task<bool> DeleteAsync(int id);
    }
}