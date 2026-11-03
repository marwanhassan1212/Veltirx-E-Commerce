
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.VariantOptionValue
{
    public class VariantOptionValueRepository : IVariantOptionValueRepository
    {
        private readonly AppDbContext _db;

        public VariantOptionValueRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Models.VariantOptionValue>> GetByIdsAsync(
            IReadOnlyCollection<int> ids)
        {
            return await _db.VariantOptionValues
                .Where(x => ids.Contains(x.Id))
                .Include(x => x.VariantOption)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<int>> GetOptionGroupIdsByProductIdAsync(int productId)
        {
            return await _db.VariantOptions
                .Where(x => x.ProductId == productId)
                .Select(x => x.Id)
                .ToListAsync();
        }
    }
}
