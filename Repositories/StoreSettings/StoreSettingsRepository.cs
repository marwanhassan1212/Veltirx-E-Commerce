using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.StoreSettings
{
    public class StoreSettingsRepository : IStoreSettingsRepository
    {
        private readonly AppDbContext _db;
        public StoreSettingsRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.StoreSettings settings)
        {
            await _db.StoreSettings.AddAsync(settings);
        }

        public async Task<Models.StoreSettings?> GetActiveAsync()
        {
            return await _db.StoreSettings.Where(x => x.IsActive)
                .AsNoTracking().OrderByDescending(x => x.UpdatedAt)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Models.StoreSettings>> GetAllAsync()
        {
            return await _db.StoreSettings.AsNoTracking()
                .OrderByDescending(x => x.UpdatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<Models.StoreSettings?> GetByIdAsync(int id)
        {
            return await _db.StoreSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(Models.StoreSettings settings)
        {
            _db.StoreSettings.Update(settings);
        }
    }
}
