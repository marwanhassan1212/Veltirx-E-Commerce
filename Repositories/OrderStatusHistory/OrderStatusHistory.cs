
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.OrderStatusHistory
{
    public class OrderStatusHistory : IOrderStatusHistory
    {
        private readonly AppDbContext _db;
        public OrderStatusHistory(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.OrderStatusHistory statusHistory)
        {
            await _db.OrderStatusHistories.AddAsync(statusHistory);
        }

        public async Task<Models.OrderStatusHistory?> GetByIdAsync(long id)
        {
            return await _db.OrderStatusHistories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.OrderStatusHistory>> GetByOrderIdAsync(int orderId)
        {
            return await _db.OrderStatusHistories.Where(x => x.OrderId == orderId)
                .AsNoTracking().OrderByDescending(x => x.ChangedAt).ThenByDescending(x => x.Id)
                .ToListAsync();
        }
    }
}
