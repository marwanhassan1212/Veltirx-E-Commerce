
using Veltrix.DTOs.OrderFulfillmentDTO;
using Veltrix.UnitOfWork;
using FulfillmentModel = Veltrix.Models.OrderFulfillment;

namespace Veltrix.Services.OrderFulfillment
{
    public class OrderFulfillmentService : IOrderFulfillmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderFulfillmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderFulfillmentDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid fulfillment ID.");

            var fulfillment =
                await _unitOfWork.OrderFulfillments.GetByIdAsync(id);

            return fulfillment == null ? null : MapToDto(fulfillment);
        }

        public async Task<OrderFulfillmentDto?> GetByFulfillmentNumberAsync(string fulfillmentNumber)

        {
            if (string.IsNullOrWhiteSpace(fulfillmentNumber))
                throw new ArgumentException("Fulfillment number is required.");

            var fulfillment =
                await _unitOfWork.OrderFulfillments.GetByFulfillmentNumberAsync(fulfillmentNumber.Trim());
            return fulfillment == null ? null : MapToDto(fulfillment);
        }

        public async Task<IReadOnlyList<OrderFulfillmentDto>> GetByOrderIdAsync(int orderId)

        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid order ID.");
            var fulfillments = await _unitOfWork.OrderFulfillments.GetByOrderIdAsync(orderId);
            return fulfillments.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<OrderFulfillmentDto>> GetAllAsync()
        {
            var fulfillments = await _unitOfWork.OrderFulfillments.GetAllAsync();
            return fulfillments.Select(MapToDto).ToList();
        }

        private static OrderFulfillmentDto MapToDto(
            FulfillmentModel fulfillment)
        {
            return new OrderFulfillmentDto
            {
                Id = fulfillment.Id,
                FulfillmentNumber = fulfillment.FulfillmentNumber,
                OrderId = fulfillment.OrderId,
                Status = fulfillment.Status,
                Notes = fulfillment.Notes,
                CreatedAt = fulfillment.CreatedAt,
                UpdatedAt = fulfillment.UpdatedAt,
                CompletedAt = fulfillment.CompletedAt,

                Items = fulfillment.Items
                    .Select(item => new FulfillmentItemDto
                    {
                        Id = item.Id,
                        FulfillmentId = item.FulfillmentId,
                        OrderItemId = item.OrderItemId,
                        Quantity = item.Quantity
                    })
                    .ToList()
            };
        }
    }
}
