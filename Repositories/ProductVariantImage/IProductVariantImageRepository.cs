using Veltrix.Models;

namespace Veltrix.Repositories.ProductVariantImage
{
    public interface IProductVariantImageRepository
    {
        Task<Models.ProductVariantImage?> GetByIdAsync(int id);
        Task<IReadOnlyList<Models.ProductVariantImage>> GetByVariantIdAsync(int variantId);
        Task AddAsync(Models.ProductVariantImage image);
        void Update(Models.ProductVariantImage image);
        void Delete(Models.ProductVariantImage image);
    }
}