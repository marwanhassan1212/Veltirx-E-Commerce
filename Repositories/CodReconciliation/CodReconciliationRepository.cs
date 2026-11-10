
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.CodReconciliation
{
    public class CodReconciliationRepository : ICodReconciliationRepository
    {
        private readonly AppDbContext _db;
        public CodReconciliationRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.CodReconciliation reconciliation)
        {
            await _db.CodReconciliations.AddAsync(reconciliation);
        }

        public async Task<IReadOnlyList<Models.CodReconciliation>> GetAllAsync()
        {
            return await _db.CodReconciliations
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Models.CodReconciliation?> GetByIdAsync(int id)
        {
            return await _db.CodReconciliations
                 .Include(x => x.Items)
                     .ThenInclude(x => x.Order)
                 .Include(x => x.ReconciledByUser)
                 .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Models.CodReconciliation?> GetByReconciliationNumberAsync(string reconciliationNumber)
        {
            return await _db.CodReconciliations
                 .Include(x => x.Items)
                     .ThenInclude(x => x.Order)
                 .Include(x => x.ReconciledByUser)
                 .FirstOrDefaultAsync(
                     x => x.ReconciliationNumber == reconciliationNumber);
        }

        public void Update(Models.CodReconciliation reconciliation)
        {
            _db.CodReconciliations.Update(reconciliation);
        }
    }
}
