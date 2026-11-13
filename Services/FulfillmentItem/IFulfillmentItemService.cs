
using Veltrix.DTOs.OrderFulfillmentDTO;

namespace Veltrix.Services.FulfillmentItem
{
    public interface IFulfillmentItemService
    {
        Task<FulfillmentItemDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<FulfillmentItemDto>> GetByFulfillmentIdAsync(int fulfillmentId);
        Task<IReadOnlyList<FulfillmentItemDto>> GetByOrderItemIdAsync(int orderItemId);

    }
}
