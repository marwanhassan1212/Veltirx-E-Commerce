using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.Product
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;
        public ProductRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Models.Product>> GetAllAsync()
        {
            return await _db.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .AsNoTracking()
                .ToListAsync();
                    
        }

        public async Task<Models.Product?> GetByIdAsync(int Id)
        {
            return await _db.Products
              .Include(x => x.Category)
              .Include(x => x.ProductImages)
              .FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Models.Product?> GetBySlugAsync(string slug)
        {
            return await _db.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Slug == slug);
        }

        public async Task<IReadOnlyList<Models.Product>> GetPublishedProductsAsync()
        {
            return await _db.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Where(x => x.IsPublished)
                .AsNoTracking()
                .ToListAsync();
        }

        public void Update(Models.Product product)
        {
            _db.Products.Update(product);
        }

        public async Task AddAsync(Models.Product product)
        {
            await _db.Products.AddAsync(product);
        }
    }
}
