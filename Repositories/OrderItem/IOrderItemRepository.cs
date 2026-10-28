namespace Veltrix.Repositories.OrderItem
{
    public interface IOrderItemRepository
    {
        Task<Models.OrderItem?> GetByIdAsync(int id);
        Task<IReadOnlyList<Models.OrderItem>> GetByOrderIdAsync(int orderId);
        Task<IReadOnlyList<Models.OrderItem>> GetByProductIdAsync(int productId);
        Task AddAsync(Models.OrderItem orderItem);
        void Update(Models.OrderItem orderItem);
        void Delete(Models.OrderItem orderItem);
    }
}
