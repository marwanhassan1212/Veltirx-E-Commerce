using Microsoft.EntityFrameworkCore;
using Veltrix.Data;
using Veltrix.Models;

namespace Veltrix.Repositories.OrderReturnItem
{
    public class OrderReturnItemRepository : IOrderReturnItemRepository
    {
        private readonly AppDbContext _db;

        public OrderReturnItemRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Models.OrderReturnItem returnItem)
        {
            await _db.OrderReturnItems.AddAsync(returnItem);
        }

        public async Task<Models.OrderReturnItem?> GetByIdAsync(int id)
        {
            return await _db.OrderReturnItems
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.OrderReturnItem>> GetByReturnIdAsync(int returnId)

        {
            return await _db.OrderReturnItems
                .Where(x => x.ReturnId == returnId)
                .AsNoTracking()
                .Include(x => x.OrderItem)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Models.OrderReturnItem>> GetByOrderItemIdAsync(int orderItemId)

        {
            return await _db.OrderReturnItems
                .Where(x => x.OrderItemId == orderItemId)
                .AsNoTracking()
                .Include(x => x.Return)
                .OrderByDescending(x => x.Return.RequestedAt)
                .ToListAsync();
        }

        public void Update(Models.OrderReturnItem returnItem)
        {
            _db.OrderReturnItems.Update(returnItem);
        }

        public void Delete(Models.OrderReturnItem returnItem)
        {
            _db.OrderReturnItems.Remove(returnItem);
        }
    }
}