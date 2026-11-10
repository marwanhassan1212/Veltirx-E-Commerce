using Veltrix.Models;

namespace Veltrix.Repositories.OrderReturnItem
{
    public interface IOrderReturnItemRepository
    {
        Task<Models.OrderReturnItem?> GetByIdAsync(int id);
        Task<IReadOnlyList<Models.OrderReturnItem>> GetByReturnIdAsync(int returnId);
        Task<IReadOnlyList<Models.OrderReturnItem>> GetByOrderItemIdAsync(int orderItemId);
        Task AddAsync(Models.OrderReturnItem returnItem);

        void Update(Models.OrderReturnItem returnItem);

        void Delete(Models.OrderReturnItem returnItem);
    }
}