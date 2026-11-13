
using Veltrix.DTOs.OrderFulfillmentDTO;
using Veltrix.UnitOfWork;
using FulfillmentItemModel = Veltrix.Models.FulfillmentItem;

namespace Veltrix.Services.FulfillmentItem
{
    public class FulfillmentItemService : IFulfillmentItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FulfillmentItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FulfillmentItemDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid fulfillment item ID.");

            var item = await _unitOfWork.FulfillmentItems.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<IReadOnlyList<FulfillmentItemDto>> GetByFulfillmentIdAsync(int fulfillmentId)
        {
            if (fulfillmentId <= 0)
                throw new ArgumentException("Invalid fulfillment ID.");
            var items = await _unitOfWork.FulfillmentItems.GetByFulfillmentIdAsync(fulfillmentId);

            return items.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<FulfillmentItemDto>> GetByOrderItemIdAsync(int orderItemId)
        {
            if (orderItemId <= 0)
                throw new ArgumentException("Invalid order item ID.");

            var items =
                await _unitOfWork.FulfillmentItems
                    .GetByOrderItemIdAsync(orderItemId);

            return items.Select(MapToDto).ToList();
        }

        private static FulfillmentItemDto MapToDto(FulfillmentItemModel item)
        {
            return new FulfillmentItemDto
            {
                Id = item.Id,
                FulfillmentId = item.FulfillmentId,
                OrderItemId = item.OrderItemId,
                Quantity = item.Quantity
            };
        }
    }
}
