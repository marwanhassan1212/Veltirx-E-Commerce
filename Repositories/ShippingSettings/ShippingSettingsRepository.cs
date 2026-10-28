
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.ShippingSettings
{
    public class ShippingSettingsRepository : IShippingSettingsRepository
    {
        private readonly AppDbContext _db;
        public ShippingSettingsRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.ShippingSettings settings)
        {
            await _db.ShippingSettings.AddAsync(settings);
        }

        public async Task<Models.ShippingSettings?> GetActiveAsync()
        {
            return await _db.ShippingSettings
             .AsNoTracking()
             .Where(x => x.IsActive)
             .OrderByDescending(x => x.UpdatedAt)
             .ThenByDescending(x => x.Id)
             .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Models.ShippingSettings>> GetAllAsync()
        {
            return await _db.ShippingSettings.AsNoTracking()
                .OrderByDescending(x => x.UpdatedAt).ThenByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<Models.ShippingSettings?> GetByIdAsync(int id)
        {
            return await _db.ShippingSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(Models.ShippingSettings settings)
        {
            _db.ShippingSettings.Update(settings);
        }
    }
}
