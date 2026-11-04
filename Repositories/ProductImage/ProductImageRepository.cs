
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

        public async Task<IReadOnlyList<Models.ProductImage>> GetByProductIdAsync(int productId)
        {
            return await _db.ProductImages
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.DisplayOrder)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public void Update(Models.ProductImage productImage)
        {
            _db.ProductImages.Update(productImage);
        }
    }
}
