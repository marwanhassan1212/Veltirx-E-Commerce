
using Veltrix.DTOs.OrderDTO;
using Veltrix.Repositories.Order;
using Veltrix.UnitOfWork;
using OrderModel = Veltrix.Models.Order;

namespace Veltrix.Services.Order
{
    public class OrderQueryService : IOrderQueryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderQueryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderDto?> GetCustomerOrderByIdAsync(string userId, int orderId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.");

            if (orderId <= 0)
                throw new ArgumentException("Invalid order ID.");

            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

            if (order == null || order.UserId != userId)
                return null;

            return MapToDto(order);
        }

        public async Task<OrderDto?> GetCustomerOrderByNumberAsync(string userId, string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.");

            if (string.IsNullOrWhiteSpace(orderNumber))
                throw new ArgumentException("Invalid order number.");

            var order = await _unitOfWork.Orders
                .GetByOrderNumberAsync(orderNumber);

            if (order == null || order.UserId != userId)
                return null;

            return MapToDto(order);
        }

        public async Task<IReadOnlyList<OrderDto>> GetCustomerOrdersAsync(string userId)

        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.");

            var orders = await _unitOfWork.Orders
                .GetByUserIdAsync(userId);

            return orders.Select(MapToDto).ToList();
        }
        public async Task<(IReadOnlyList<OrderSummaryDto> Items, int TotalCount)> GetPagedOrdersAsync(OrderFilterParameters filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            filters.PageNumber = Math.Max(1, filters.PageNumber);
            filters.PageSize = Math.Clamp(filters.PageSize, 1, 100);

            if (filters.FromDate.HasValue &&
                filters.ToDate.HasValue &&
                filters.FromDate.Value.Date > filters.ToDate.Value.Date)
            {
                throw new ArgumentException(
                    "FromDate cannot be later than ToDate.");
            }

            var result = await _unitOfWork.Orders.GetPagedAsync(filters);

            var items = result.Items.Select(order => new OrderSummaryDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt
            }).ToList();

            return (items, result.TotalCount);
        }

        private static OrderDto MapToDto(OrderModel order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                TotalAmount = order.TotalAmount,

                CouponId = order.CouponId,

                ShippingRecipientName = order.ShippingRecipientName,
                ShippingPhoneNumber = order.ShippingPhoneNumber,
                ShippingAddressLine1 = order.ShippingAddressLine1,
                ShippingAddressLine2 = order.ShippingAddressLine2,
                ShippingCity = order.ShippingCity,
                ShippingGovernorate = order.ShippingGovernorate,
                ShippingPostalCode = order.ShippingPostalCode,
                ShippingInstructions = order.ShippingInstructions,

                CancelReason = order.CancelReason,

                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                ConfirmedAt = order.ConfirmedAt,
                ShippedAt = order.ShippedAt,
                DeliveredAt = order.DeliveredAt,
                CancelledAt = order.CancelledAt,

                Items = order.OrderItems.Select(item => new
                    Veltrix.DTOs.OrderItemDTO.OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductVariant.ProductId,
                    ProductName = item.ProductName,
                    SKU = item.SKU,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    LineTotal = item.LineTotal,
                    ProductVariantId = item.ProductVariantId,
                    VariantName = item.VariantName,
                    VariantOptionsSnapshot = item.VariantOptionsSnapshot,
                    CancelledQuantity = item.CancelledQuantity,
                    FulfilledQuantity = item.FulfilledQuantity
                }).ToList()
            };
        }
    }
}
