using Veltrix.DTOs.CategoryDTO;

namespace Veltrix.Services.Category
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync();
        Task<IReadOnlyList<CategoryDto>> GetActiveCategoriesAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto?> GetBySlugAsync(string slug);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryDto?> UpdateAsync(int id , UpdateCategoryDto dto);
      




    }
}
