
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;
using Veltrix.Models;

namespace Veltrix.Repositories.VariantOptionValue
{
    public class VariantOptionValueRepository : IVariantOptionValueRepository
    {
        private readonly AppDbContext _db;

        public VariantOptionValueRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Models.VariantOptionValue optionValue)
        {
            await _db.VariantOptionValues.AddAsync(optionValue);
        }

        public void Delete(Models.VariantOptionValue optionValue)
        {
            _db.VariantOptionValues.Remove(optionValue);
        }

        public async Task<Models.VariantOptionValue?> GetByIdAsync(int id)
        {
            return await _db.VariantOptionValues
                .Include(x => x.VariantOption)
                .Include(x => x.ProductVariantOptions)
                .FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<IReadOnlyList<Models.VariantOptions>> GetGroupsWithValuesByProductIdAsync(int productId)
        {
            return await _db.VariantOptions.Where(x => x.ProductId == productId)
                 .Include(x => x.Values.OrderBy(x => x.DisplayOrder))
                 .OrderBy(x => x.DisplayOrder).AsNoTracking()
                 .ToListAsync();
        }

        public async Task<IReadOnlyList<int>> GetOptionGroupIdsByProductIdAsync(int productId)
        {
            return await _db.VariantOptions
                .Where(x => x.ProductId == productId)
                .Select(x => x.Id)
                .ToListAsync();
        }

        public void Update(Models.VariantOptionValue optionValue)
        {
            _db.VariantOptionValues.Update(optionValue);
        }

   
    }
}
