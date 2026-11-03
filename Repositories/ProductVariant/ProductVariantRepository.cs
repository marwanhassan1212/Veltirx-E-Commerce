
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;
using Veltrix.Models;

namespace Veltrix.Repositories.ProductVariant
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly AppDbContext _db;
        public ProductVariantRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.ProductVariant variant)
        {
            await _db.ProductVariants.AddAsync(variant);
        }

        public async Task AddOptionAsync(Models.ProductVariantOption option)
        {
            await _db.ProductVariantOptions.AddAsync(option);
        }

        public void Delete(Models.ProductVariant variant)
        {
            _db.ProductVariants.Remove(variant);
        }

           public async Task<Models.ProductVariant?> GetByIdAsync(int id)
        {
            return await _db.ProductVariants
                .Include(x => x.Product)
                .Include(x => x.VariantOptions)
                    .ThenInclude(x => x.VariantOptionValue)
                .Include(x => x.OrderItems)
                .Include(x => x.CartItems)
                .Include(x => x.Inventory)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.ProductVariant>> GetByProductIdAsync(int productId)
        {
            return await _db.ProductVariants
             .Where(x => x.ProductId == productId)
             .Include(x => x.VariantOptions)
                 .ThenInclude(x => x.VariantOptionValue)
             .AsNoTracking()
             .ToListAsync();
        }

        public async Task<IReadOnlyList<Models.ProductVariant>> GetByProductIdForUpdateAsync(int productId)
        {
            return await _db.ProductVariants
                .Where(x => x.ProductId == productId)
                .ToListAsync();
        }

        public async Task<Models.ProductVariant?> GetBySkuAsync(string sku)
        {
            return await _db.ProductVariants.FirstOrDefaultAsync(x => x.SKU == sku);
        }

        public async Task RemoveOptionsNotInAsync(int variantId, IReadOnlyCollection<int> selectedValueIds)
        {
            var optionsToRemove = await _db.ProductVariantOptions
                .Where(x => x.ProductVariantId == variantId &&
                            !selectedValueIds.Contains(x.VariantOptionValueId))
                .ToListAsync();

            _db.ProductVariantOptions.RemoveRange(optionsToRemove);
        }

        public void Update(Models.ProductVariant variant)
        {
            _db.ProductVariants.Update(variant);
        }
    }
}
