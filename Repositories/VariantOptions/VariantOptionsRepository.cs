using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.VariantOptions
{
    public class VariantOptionsRepository : IVariantOptionsRepository
    {
        private readonly AppDbContext _db;

        public VariantOptionsRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Models.VariantOptions?> GetByIdAsync(int id)
        {
            return await _db.VariantOptions
                .Include(x => x.Values)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.VariantOptions>>
            GetByProductIdAsync(int productId)
        {
            return await _db.VariantOptions
                .Where(x => x.ProductId == productId)
                .Include(x => x.Values.OrderBy(v => v.DisplayOrder))
                .OrderBy(x => x.DisplayOrder)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Models.VariantOptions option)
        {
            await _db.VariantOptions.AddAsync(option);
        }

        public void Update(Models.VariantOptions option)
        {
            _db.VariantOptions.Update(option);
        }

        public void Delete(Models.VariantOptions option)
        {
            _db.VariantOptions.Remove(option);
        }
    }
}