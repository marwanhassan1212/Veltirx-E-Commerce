using Veltrix.DTOs.OrderItemDTO;
using Veltrix.Services.Order;
using Veltrix.UnitOfWork;

namespace Veltrix.Services.OrderItem
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<OrderItemDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid order item ID.");

            var orderItem = await _unitOfWork.OrderItems.GetByIdAsync(id);

            if (orderItem == null)
                return null;

            return MapToDto(orderItem);
        }

        public async Task<IReadOnlyList<OrderItemDto>> GetByOrderIdAsync(int orderId, string? userId = null)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid order ID.");

            if (userId != null)
            {
                if (string.IsNullOrWhiteSpace(userId))
                    throw new ArgumentException("Invalid user ID.");

                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

                if (order == null || order.UserId != userId)
                    return Array.Empty<OrderItemDto>();
            }

            var orderItems =
                await _unitOfWork.OrderItems.GetByOrderIdAsync(orderId);

            return orderItems.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<OrderItemDto>> GetByProductIdAsync(int productId)
        {
            if (productId <= 0)
                throw new ArgumentException("Invalid product ID.");

            var orderItems =
                await _unitOfWork.OrderItems.GetByProductIdAsync(productId);

            return orderItems.Select(MapToDto).ToList();
        }

        private static OrderItemDto MapToDto(Models.OrderItem orderItem)
        {
            return new OrderItemDto
            {
                Id = orderItem.Id,
                ProductId = orderItem.ProductVariant.ProductId,
                ProductName = orderItem.ProductName,
                SKU = orderItem.SKU,
                UnitPrice = orderItem.UnitPrice,
                Quantity = orderItem.Quantity,
                LineTotal = orderItem.LineTotal,
                ProductVariantId = orderItem.ProductVariantId,
                VariantName = orderItem.VariantName,
                VariantOptionsSnapshot = orderItem.VariantOptionsSnapshot,
                CancelledQuantity = orderItem.CancelledQuantity,
                FulfilledQuantity = orderItem.FulfilledQuantity
            };
        }
    }
}
