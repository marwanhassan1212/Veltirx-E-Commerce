using Veltrix.Models;

namespace Veltrix.Repositories.Category
{
    public interface ICategoryRepository
    {
        Task<Models.Category?> GetByIdAsync(int Id);
        Task<Models.Category?> GetBySlugAsync(string slug);
        Task<IReadOnlyList<Models.Category>> GetAllAsync();
        Task<IReadOnlyList<Models.Category>> GetActiveCategoriesAsync();
        Task AddAsync(Models.Category category);
        void Update(Models.Category category);

    }
}
