using Veltrix.DTOs.Common;
using Veltrix.DTOs.ProductDTO;
using Veltrix.DTOs.Storefront;
using Veltrix.Repositories.Product;

namespace Veltrix.Services.Product
{
    public interface IProductService
    {
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto?> GetBySlugAsync(string slug);
        Task<IReadOnlyList<ProductDto>> GetAllAsync();
        Task<IReadOnlyList<ProductDto>> GetPublishedProductsAsync();
        Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
        Task<StorefrontProductDto?> GetStorefrontProductAsync(string slug);
        Task<ProductDto?> UpdateAsync(int id , UpdateProductDto updateProductDto);
        Task<bool> PublishAsync(int id);
        Task<bool> UnpublishAsync(int id);
        Task<PagedResultDto<ProductDto>> GetPagedAsync(int pageNumber, int pageSize,
            string? search, string? status);

    }
}
