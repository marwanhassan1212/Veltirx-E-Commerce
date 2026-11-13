
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

        public async Task<(IReadOnlyList<Models.OrderFulfillment> Items, int TotalCount)> GetPagedAsync(OrderFulfillmentFilterParameters filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            var pageNumber = Math.Max(1, filters.PageNumber);
            var pageSize = Math.Clamp(filters.PageSize, 1, 100);
            var query = _db.OrderFulfillments.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.FulfillmentNumber))
            {
                var number = filters.FulfillmentNumber.Trim();
                query = query.Where(x => x.FulfillmentNumber.Contains(number));

            }

            if (filters.OrderId.HasValue)
            {
                query = query.Where(x => x.OrderId == filters.OrderId.Value);
            }

            if (filters.Status.HasValue)
            {
                query = query.Where(x => x.Status == filters.Status.Value);
            }

            if (filters.FromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= filters.FromDate.Value.Date);

            }

            if (filters.ToDate.HasValue)
            {
                var toDate = filters.ToDate.Value.Date.AddDays(1);
                query = query.Where(x => x.CreatedAt < toDate);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

    }
}
