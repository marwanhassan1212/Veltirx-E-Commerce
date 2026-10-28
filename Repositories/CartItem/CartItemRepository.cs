
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Veltrix.Data;

namespace Veltrix.Repositories.CartItem
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _db;
        public CartItemRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.CartItem cartItem)
        {
            await _db.CartItems.AddAsync(cartItem);
        }

        public void Delete(Models.CartItem cartItem)
        {
            _db.CartItems.Remove(cartItem);
        }

        public async Task<Models.CartItem?> GetByCartAndProductAsync(int cartId, int productId)
        {
            return await _db.CartItems.
                FirstOrDefaultAsync(x => x.CartId == cartId && x.ProductId == productId);
        }

        public async Task<IReadOnlyList<Models.CartItem>> GetByCartIdAsync(int cartId)
        {
            return await _db.CartItems.Where(x => x.CartId == cartId)
                .AsNoTracking().OrderBy(x => x.AddedAt).ToListAsync();
        }

        public async Task<Models.CartItem?> GetByIdAsync(int id)
        {
            return await _db.CartItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(Models.CartItem cartItem)
        {
            _db.CartItems.Update(cartItem);
        }
    }
}
