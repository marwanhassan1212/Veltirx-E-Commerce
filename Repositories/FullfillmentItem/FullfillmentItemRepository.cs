using Microsoft.EntityFrameworkCore;
using Veltrix.Data;
using Veltrix.Repositories.FulfillmentItem;

namespace Veltrix.Repositories.FullfillmentItem
{
    public class FullfillmentItemRepository : IFulfillmentItemRepository
    {
        private readonly AppDbContext _db;
        public FullfillmentItemRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.FulfillmentItem fullfillmentItem)
        {
            await _db.FulfillmentItems.AddAsync(fullfillmentItem);
        }

        public void Delete(Models.FulfillmentItem fullfillmentItem)
        {
            _db.FulfillmentItems.Remove(fullfillmentItem);
        }

        public async Task<IReadOnlyList<Models.FulfillmentItem>> GetByFulfillmentIdAsync(int fullfillmentId)
        {
            return await _db.FulfillmentItems
              .Where(x => x.FulfillmentId == fullfillmentId)
              .AsNoTracking()
              .ToListAsync();
        }

        public async Task<Models.FulfillmentItem?> GetByIdAsync(int id)
        {
            return await _db.FulfillmentItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.FulfillmentItem>> GetByOrderItemIdAsync(int orderItemId)
        {
            return await _db.FulfillmentItems.Where(x => x.OrderItemId == orderItemId)
                 .AsNoTracking().ToListAsync();
        }

        public void Update(Models.FulfillmentItem fullfillmentItem)
        {
            _db.FulfillmentItems.Update(fullfillmentItem);
        }
    }
}
