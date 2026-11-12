
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Veltrix.Data;

namespace Veltrix.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;
        public OrderRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.Order order)
        {
            await _db.Orders.AddAsync(order);
        }

        public async Task<(IReadOnlyList<Models.Order> Items, int TotalCount)>
           GetPagedAsync(OrderFilterParameters filters)
        {
            var query = _db.Orders.AsNoTracking();

            // Filtering

            if (!string.IsNullOrWhiteSpace(filters.OrderNumber))
            {
                query = query.Where(x =>
                    x.OrderNumber.Contains(filters.OrderNumber));
            }

            if (!string.IsNullOrWhiteSpace(filters.UserId))
            {
                query = query.Where(x =>
                    x.UserId == filters.UserId);
            }

            if (filters.OrderStatus.HasValue)
            {
                query = query.Where(x =>
                    x.OrderStatus == filters.OrderStatus.Value);
            }

            if (filters.FromDate.HasValue)
            {
                query = query.Where(x =>
                    x.CreatedAt >= filters.FromDate.Value.Date);
            }

            if (filters.ToDate.HasValue)
            {
                var toDate = filters.ToDate.Value.Date.AddDays(1);

                query = query.Where(x =>
                    x.CreatedAt < toDate);
            }

           
            var totalCount = await query.CountAsync();

            // Pagination

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Models.Order?> GetByIdAsync(int id)
        {
            return await _db.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.ProductVariant)
                    .ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Models.Order?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _db.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.ProductVariant)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);
        }

        public async Task<IReadOnlyList<Models.Order>> GetByUserIdAsync(string userId)
        {
            return await _db.Orders
             .AsNoTracking()
             .Include(o => o.OrderItems)
                 .ThenInclude(i => i.ProductVariant)
             .Where(o => o.UserId == userId)
             .OrderByDescending(o => o.CreatedAt)
             .ToListAsync();

        }

        public void Update(Models.Order order)
        {
            _db.Orders.Update(order);
        }
    }
}
