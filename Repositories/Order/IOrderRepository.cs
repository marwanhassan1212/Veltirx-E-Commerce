namespace Veltrix.Repositories.Order
{
    public interface IOrderRepository
    {
        Task<Models.Order?> GetByIdAsync(int id);
        Task<Models.Order?> GetByOrderNumberAsync(string orderNumber);
        Task<IReadOnlyList<Models.Order>> GetByUserIdAsync(string userId);
        Task<(IReadOnlyList<Models.Order> Items, int TotalCount)> GetPagedAsync(OrderFilterParameters filters);
        Task AddAsync(Models.Order order);
        void Update(Models.Order order);

    }
}
