namespace Veltrix.Repositories.Cart
{
    public interface ICartRepository
    {
        Task<Models.Cart?> GetByUserIdAsync(string userId);
        Task AddAsync(Models.Cart cart);
        void Update(Models.Cart cart);
        void Delete(Models.Cart cart);

    }
}
