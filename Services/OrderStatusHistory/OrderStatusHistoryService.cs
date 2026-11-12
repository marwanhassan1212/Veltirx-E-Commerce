using Veltrix.DTOs.OrderStatusDTO;
using Veltrix.UnitOfWork;
using OrderStatusHistoryModel = Veltrix.Models.OrderStatusHistory;
namespace Veltrix.Services.OrderStatusHistory
{
    public class OrderStatusHistoryService : IOrderStatusHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderStatusHistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IReadOnlyList<OrderStatusHistoryDto>> GetByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid order ID.");

            var history = await _unitOfWork.OrdersStatusHistory
                .GetByOrderIdAsync(orderId);

            return history.Select(MapToDto).ToList();
        }

        private static OrderStatusHistoryDto MapToDto(OrderStatusHistoryModel history)

        {
            return new OrderStatusHistoryDto
            {
                Id = history.Id,
                OrderId = history.OrderId,
                PreviousStatus = history.PreviousStatus,
                NewStatus = history.NewStatus,
                Note = history.Note,
                ChangedAt = history.ChangedAt,
                ChangedByUserId = history.ChangedByUserId,
               
            };
        }
    }
}
