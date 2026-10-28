namespace Veltrix.Repositories.ProductImage
{
    public interface IProductImageRepository
    {
        Task<Models.ProductImage?> GetByIdAsync(int id);
        Task<IReadOnlyList<Models.ProductImage>> GetByProductId(int productId);
        Task AddAsync(Models.ProductImage productImage);
        void Update(Models.ProductImage productImage);
        void Delete(Models.ProductImage productImage);

    }
}
