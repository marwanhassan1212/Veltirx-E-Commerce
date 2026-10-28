namespace Veltrix.Repositories.OrderStatusHistory
{
    public interface IOrderStatusHistory
    {
        Task<Models.OrderStatusHistory?> GetByIdAsync(long id);
        Task<IReadOnlyList<Models.OrderStatusHistory>> GetByOrderIdAsync(int orderId);
        Task AddAsync(Models.OrderStatusHistory statusHistory);
    }
}
