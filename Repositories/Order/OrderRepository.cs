
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

        public async Task<IReadOnlyList<Models.Order>> GetAllAsync()
        {
            return await _db.Orders.AsNoTracking().OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Models.Order?> GetByIdAsync(int id)
        {
            return await _db.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Models.Order?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _db.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);
        }

        public async Task<IReadOnlyList<Models.Order>> GetByUserIdAsync(string userId)
        {
            return await _db.Orders.Where(x => x.UserId == userId)
                .AsNoTracking().OrderByDescending(x => x.CreatedAt).ToListAsync();
                
        }

        public void Update(Models.Order order)
        {
            _db.Orders.Update(order);
        }
    }
}
