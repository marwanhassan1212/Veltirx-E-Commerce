using Microsoft.EntityFrameworkCore;
using Veltrix.Data;
using Veltrix.Models;

namespace Veltrix.Repositories.OrderReturn
{
    public class OrderReturnRepository : IOrderReturnRepository
    {
        private readonly AppDbContext _db;

        public OrderReturnRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Models.OrderReturn orderReturn)
        {
            await _db.OrderReturns.AddAsync(orderReturn);
        }

        public async Task<IReadOnlyList<Models.OrderReturn>> GetAllAsync()
        {
            return await _db.OrderReturns
                .AsNoTracking()
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        public async Task<Models.OrderReturn?> GetByIdAsync(int id)
        {
            return await _db.OrderReturns
                .Include(x => x.Items)
                    .ThenInclude(x => x.OrderItem)
                .Include(x => x.Refunds)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Models.OrderReturn?> GetByReturnNumberAsync(
            string returnNumber)
        {
            return await _db.OrderReturns
                .Include(x => x.Items)
                    .ThenInclude(x => x.OrderItem)
                .Include(x => x.Refunds)
                .FirstOrDefaultAsync(x => x.ReturnNumber == returnNumber);
        }

        public async Task<IReadOnlyList<Models.OrderReturn>> GetByOrderIdAsync(
            int orderId)
        {
            return await _db.OrderReturns
                .Where(x => x.OrderId == orderId)
                .AsNoTracking()
                .Include(x => x.Items)
                    .ThenInclude(x => x.OrderItem)
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync();
        }

        public void Update(Models.OrderReturn orderReturn)
        {
            _db.OrderReturns.Update(orderReturn);
        }
    }
}