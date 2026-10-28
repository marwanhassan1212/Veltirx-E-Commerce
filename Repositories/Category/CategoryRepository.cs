
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

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
            return await _db.Categories.AsNoTracking()
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Models.Category?> GetByIdAsync(int Id)
        {
            return await _db.Categories.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Models.Category?> GetBySlugAsync(string slug)
        {
            return await _db.Categories.FirstOrDefaultAsync(x => x.Slug == slug);
        }

        public void Update(Models.Category category)
        {
            _db.Categories.Update(category);
        }
    }
}
