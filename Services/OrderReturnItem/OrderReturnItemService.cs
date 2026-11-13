
using Veltrix.DTOs.OrderReturnDTO;
using Veltrix.UnitOfWork;
using OrderReturnItemModel = Veltrix.Models.OrderReturnItem;

namespace Veltrix.Services.OrderReturnItem
{
    public class OrderReturnItemService : IOrderReturnItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderReturnItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderReturnItemDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid return item ID.");

            var item =
                await _unitOfWork.OrderReturnItems.GetByIdAsync(id);

            return item == null ? null : MapToDto(item);
        }

        public async Task<IReadOnlyList<OrderReturnItemDto>> GetByReturnIdAsync(int returnId)
        {
            if (returnId <= 0)
                throw new ArgumentException("Invalid return ID.");

            var items =
                await _unitOfWork.OrderReturnItems.GetByReturnIdAsync(returnId);

            return items.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<OrderReturnItemDto>> GetByOrderItemIdAsync(int orderItemId)
        {
            if (orderItemId <= 0)
                throw new ArgumentException("Invalid order item ID.");

            var items =
                await _unitOfWork.OrderReturnItems
                    .GetByOrderItemIdAsync(orderItemId);

            return items.Select(MapToDto).ToList();
        }

        private static OrderReturnItemDto MapToDto(OrderReturnItemModel item)
        {
            return new OrderReturnItemDto
            {
                Id = item.Id,
                ReturnId = item.ReturnId,
                OrderItemId = item.OrderItemId,
                Quantity = item.Quantity,
                ConditionNotes = item.ConditionNotes
            };
        }
    }
}
