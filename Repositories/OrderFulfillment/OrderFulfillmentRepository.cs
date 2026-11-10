
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.OrderFulfillment
{
    public class OrderFulfillmentRepository : IOrderFulfillmentRepository
    {
        private readonly AppDbContext _db;
        public OrderFulfillmentRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.OrderFulfillment fulfillment)
        {
            await _db.OrderFulfillments.AddAsync(fulfillment);
        }

        public async Task<IReadOnlyList<Models.OrderFulfillment>> GetAllAsync()
        {
            return await _db.OrderFulfillments.AsNoTracking().OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Models.OrderFulfillment?> GetByFulfillmentNumberAsync(string fulfillmentNumber)
        {
            return await _db.OrderFulfillments.Include(x => x.Items)
                 .ThenInclude(x => x.OrderItem)
                 .ThenInclude(x => x.ProductVariant)
                 .ThenInclude(x => x.Product)
                 .FirstOrDefaultAsync(x => x.FulfillmentNumber == fulfillmentNumber);
        }

        public async Task<Models.OrderFulfillment?> GetByIdAsync(int id)
        {
            return await _db.OrderFulfillments
                .Include(x => x.Items)
                    .ThenInclude(x => x.OrderItem)
                        .ThenInclude(x => x.ProductVariant)
                            .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.OrderFulfillment>> GetByOrderIdAsync(int orderId)
        {
            return await _db.OrderFulfillments
               .Where(x => x.OrderId == orderId)
               .AsNoTracking()
               .Include(x => x.Items)
                   .ThenInclude(x => x.OrderItem)
               .OrderByDescending(x => x.CreatedAt)
               .ToListAsync();
        }

        public void Update(Models.OrderFulfillment fulfillment)
        {
            _db.OrderFulfillments.Update(fulfillment);
        }
    }
}
