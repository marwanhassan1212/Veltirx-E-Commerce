namespace Veltrix.Repositories.OrderFulfillment
{
    public interface IOrderFulfillmentRepository
    {
        Task<Models.OrderFulfillment?> GetByIdAsync(int id);
        Task<Models.OrderFulfillment?> GetByFulfillmentNumberAsync(string fulfillmentNumber);
        Task<IReadOnlyList<Models.OrderFulfillment>> GetByOrderIdAsync(int orderId);
        Task<IReadOnlyList<Models.OrderFulfillment>> GetAllAsync();
        Task AddAsync(Models.OrderFulfillment fulfillment);
        void Update(Models.OrderFulfillment fulfillment);
    }
}
