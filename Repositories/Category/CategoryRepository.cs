
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;
using Veltrix.DTOs.CategoryDTO;

namespace Veltrix.Repositories.Category
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _db;
        public CategoryRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.Category category)
        {
            await _db.Categories.AddAsync(category);
        }

        public async Task<IReadOnlyList<Models.Category>> GetActiveCategoriesAsync()
        {
            return await _db.Categories.Where(x => x.IsActive)
                .AsNoTracking()
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Models.Category>> GetAllAsync()
        {
            return await _db.Categories.OrderBy(x => x.DisplayOrder).ToListAsync();


        }

        public async Task<Models.Category?> GetByIdAsync(int Id)
        {
            return await _db.Categories.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Models.Category?> GetBySlugAsync(string slug)
        {
            return await _db.Categories.FirstOrDefaultAsync(x => x.Slug == slug);
        }

        public async Task<bool> HasActiveSubCategoriesAsync(int categoryId)
        {
            return await _db.Categories.AnyAsync(x => x.ParentCategoryId == categoryId
            && x.IsActive);
        }

        public async Task<bool> HasPublishedProductsAsync(int categoryId)
        {
            return await _db.Products.AnyAsync(x => x.CategoryId == categoryId
            && x.IsPublished);
        }

        public void Update(Models.Category category)
        {
            _db.Categories.Update(category);
        }

        public async Task<IReadOnlyList<CategoryDto>> GetCategoriesForListAsync()
        {
            return await _db.Categories
                .AsNoTracking()
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    ImageUrl = c.ImageUrl,
                    Description = c.Description,
                    DisplayOrder = c.DisplayOrder,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.ParentCategory != null
                        ? c.ParentCategory.Name
                        : null,
                    ProductsCount = c.Products.Count()
                })
                .ToListAsync();
        }
    }
}
