using Veltrix.DTOs.CategoryDTO;
using Veltrix.Models;

namespace Veltrix.Repositories.Category
{
    public interface ICategoryRepository
    {
        Task<Models.Category?> GetByIdAsync(int Id);
        Task<Models.Category?> GetBySlugAsync(string slug);
        Task<IReadOnlyList<Models.Category>> GetAllAsync();
        Task<IReadOnlyList<CategoryDto>> GetCategoriesForListAsync();
        Task<IReadOnlyList<Models.Category>> GetActiveCategoriesAsync();
        Task AddAsync(Models.Category category);
        void Update(Models.Category category);
        Task<bool> HasActiveSubCategoriesAsync(int categoryId);
        Task<bool> HasPublishedProductsAsync(int categoryId);

    }
}
