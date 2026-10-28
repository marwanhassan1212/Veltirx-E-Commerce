
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.ProductImage
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _db;
        public ProductImageRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.ProductImage productImage)
        {
            await _db.ProductImages.AddAsync(productImage);
        }

        public void Delete(Models.ProductImage productImage)
        {
            _db.ProductImages.Remove(productImage);
        }

        public async Task<Models.ProductImage?> GetByIdAsync(int id)
        {
            return await _db.ProductImages.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.ProductImage>> GetByProductId(int productId)
        {
            return await _db.ProductImages.AsNoTracking()
                .Where(x => x.ProductId == productId)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        public void Update(Models.ProductImage productImage)
        {
            _db.ProductImages.Update(productImage);
        }
    }
}
