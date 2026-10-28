using Veltrix.Models;

namespace Veltrix.Repositories.Product
{
    public interface IProductRepository
    {
        Task<Models.Product?> GetByIdAsync(int Id);
        Task<Models.Product?> GetBySlugAsync(string slug);
        Task<IReadOnlyList<Models.Product>> GetAllAsync();
        Task<IReadOnlyList<Models.Product>> GetPublishedProductsAsync();
        Task AddAsync(Models.Product product);
        void Update(Models.Product product);
    }
}
