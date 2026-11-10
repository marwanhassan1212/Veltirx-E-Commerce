using Veltrix.Models;

namespace Veltrix.Repositories.FulfillmentItem
{
    public interface IFulfillmentItemRepository
    {
        Task<Models.FulfillmentItem?> GetByIdAsync(int id);
        Task<IReadOnlyList<Models.FulfillmentItem>> GetByFulfillmentIdAsync(int fullfillmentId);
        Task<IReadOnlyList<Models.FulfillmentItem>> GetByOrderItemIdAsync(int orderItemId);
        Task AddAsync(Models.FulfillmentItem fullfillmentItem);
        void Update(Models.FulfillmentItem fullfillmentItem);
        void Delete(Models.FulfillmentItem fullfillmentItem);
    }
}