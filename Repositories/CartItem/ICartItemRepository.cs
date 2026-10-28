namespace Veltrix.Repositories.CartItem
{
    public interface ICartItemRepository
    {
        Task<Models.CartItem?> GetByIdAsync(int id);
        Task<Models.CartItem?> GetByCartAndProductAsync(int cartId, int productId);
        Task<IReadOnlyList<Models.CartItem>> GetByCartIdAsync(int cartId);
        Task AddAsync(Models.CartItem cartItem);
        void Update(Models.CartItem cartItem);
        void Delete(Models.CartItem cartItem);

    }
}
