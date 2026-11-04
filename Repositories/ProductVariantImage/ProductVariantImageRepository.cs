using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.ProductVariantImage
{
    public class ProductVariantImageRepository
        : IProductVariantImageRepository
    {
        private readonly AppDbContext _db;

        public ProductVariantImageRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Models.ProductVariantImage?> GetByIdAsync(int id)
        {
            return await _db.ProductVariantImages
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.ProductVariantImage>>
            GetByVariantIdAsync(int variantId)
        {
            return await _db.ProductVariantImages
                .Where(x => x.ProductVariantId == variantId)
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.DisplayOrder)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public async Task AddAsync(Models.ProductVariantImage image)
        {
            await _db.ProductVariantImages.AddAsync(image);
        }

        public void Update(Models.ProductVariantImage image)
        {
            _db.ProductVariantImages.Update(image);
        }

        public void Delete(Models.ProductVariantImage image)
        {
            _db.ProductVariantImages.Remove(image);
        }
    }
}