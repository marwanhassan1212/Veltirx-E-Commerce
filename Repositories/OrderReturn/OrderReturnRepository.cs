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
                .Include(x => x.Items)
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


        public async Task<(IReadOnlyList<Models.OrderReturn> Items, int TotalCount)>
            GetPagedAsync(OrderReturnFilterParameters filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            var pageNumber = Math.Max(1, filters.PageNumber);
            var pageSize = Math.Clamp(filters.PageSize, 1, 100);

            var query = _db.OrderReturns
                .AsNoTracking()
                .Include(x => x.Items)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(filters.ReturnNumber))
            {
                var returnNumber = filters.ReturnNumber.Trim();

                query = query.Where(x =>
                    x.ReturnNumber.Contains(returnNumber));
            }

            // Filtering
            if (filters.OrderId.HasValue)
            {
                query = query.Where(x =>
                    x.OrderId == filters.OrderId.Value);
            }

            if (filters.Status.HasValue)
            {
                query = query.Where(x =>
                    x.Status == filters.Status.Value);
            }

            if (filters.FromDate.HasValue)
            {
                query = query.Where(x =>
                    x.RequestedAt >= filters.FromDate.Value.Date);
            }

            if (filters.ToDate.HasValue)
            {
                var toDate = filters.ToDate.Value.Date.AddDays(1);

                query = query.Where(x =>
                    x.RequestedAt < toDate);
            }

            // Total Count
            var totalCount = await query.CountAsync();

            // Pagination
            var items = await query
                .OrderByDescending(x => x.RequestedAt)
                .ThenByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

    }
}