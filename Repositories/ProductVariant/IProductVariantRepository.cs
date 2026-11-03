namespace Veltrix.Repositories.ProductVariant
{
    public interface IProductVariantRepository
    {
        Task<Models.ProductVariant?> GetByIdAsync(int id);
        Task<IReadOnlyList<Models.ProductVariant>> GetByProductIdAsync(int productId);
        Task<Models.ProductVariant?> GetBySkuAsync(string sku);
        Task<IReadOnlyList<Models.ProductVariant>> GetByProductIdForUpdateAsync(int productId);
        Task RemoveOptionsNotInAsync(int variantId, IReadOnlyCollection<int> selectedValueIds);
        Task AddOptionAsync(Models.ProductVariantOption option);
        Task AddAsync(Models.ProductVariant variant);
        void Update(Models.ProductVariant variant);
        void Delete(Models.ProductVariant variant);
    }
}
