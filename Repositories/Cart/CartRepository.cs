using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Veltrix.Data;

namespace Veltrix.Repositories.Cart
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _db;
        public CartRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Models.Cart cart)
        {
            await _db.Carts.AddAsync(cart);
        }

        public void Delete(Models.Cart cart)
        {
             _db.Carts.Remove(cart);
        }

        public async Task<Models.Cart?> GetByUserIdAsync(string userId)
        {
            return await _db.Carts
                .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public void Update(Models.Cart cart)
        {
            _db.Carts.Update(cart);
        }
    }
}
