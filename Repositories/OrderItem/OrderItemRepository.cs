
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.OrderItem
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _db;
        public OrderItemRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.OrderItem orderItem)
        {
            await _db.OrderItems.AddAsync(orderItem);
        }

        public void Delete(Models.OrderItem orderItem)
        {
            _db.OrderItems.Remove(orderItem);
        }

        public async Task<Models.OrderItem?> GetByIdAsync(int id)
        {
            return await _db.OrderItems
                .Include(x => x.ProductVariant)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.OrderItem>> GetByOrderIdAsync(int orderId)
        {
            return await _db.OrderItems
                .AsNoTracking()
                .Include(x => x.ProductVariant)
                .Where(x => x.OrderId == orderId)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Models.OrderItem>> GetByProductIdAsync(int productId)
        {
            return await _db.OrderItems
            .AsNoTracking()
            .Include(x => x.ProductVariant)
            .Where(x => x.ProductVariant.ProductId == productId)
            .OrderByDescending(x => x.Order.CreatedAt)
            .ToListAsync();
        }

        public void Update(Models.OrderItem orderItem)
        {
            _db.OrderItems.Update(orderItem);
        }
    }
}
