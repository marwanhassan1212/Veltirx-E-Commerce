
using Veltrix.DTOs.OrderReturnDTO;
using Veltrix.Repositories.OrderReturn;
using Veltrix.UnitOfWork;
using OrderReturnModel = Veltrix.Models.OrderReturn;

namespace Veltrix.Services.OrderReturn
{
    public class OrderReturnService : IOrderReturnService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderReturnService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderReturnDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid return ID.");

            var orderReturn =
                await _unitOfWork.OrderReturns.GetByIdAsync(id);

            return orderReturn == null ? null : MapToDto(orderReturn);
        }

        public async Task<OrderReturnDto?> GetByReturnNumberAsync(string returnNumber)

        {
            if (string.IsNullOrWhiteSpace(returnNumber))
                throw new ArgumentException("Return number is required.");

            var orderReturn = await _unitOfWork.OrderReturns.GetByReturnNumberAsync(returnNumber.Trim());
            return orderReturn == null ? null : MapToDto(orderReturn);
        }

        public async Task<IReadOnlyList<OrderReturnDto>> GetByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid order ID.");

            var returns =
                await _unitOfWork.OrderReturns.GetByOrderIdAsync(orderId);

            return returns.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<OrderReturnDto>> GetAllAsync()
        {
            var returns = await _unitOfWork.OrderReturns.GetAllAsync();
            return returns.Select(MapToDto).ToList();
        }

        private static OrderReturnDto MapToDto(OrderReturnModel orderReturn)
        {
            return new OrderReturnDto
            {
                Id = orderReturn.Id,
                ReturnNumber = orderReturn.ReturnNumber,
                OrderId = orderReturn.OrderId,
                Status = orderReturn.Status,
                Reason = orderReturn.Reason,
                CustomerNotes = orderReturn.CustomerNotes,
                AdminNotes = orderReturn.AdminNotes,
                RequestedAt = orderReturn.RequestedAt,
                ApprovedAt = orderReturn.ApprovedAt,
                ReceivedAt = orderReturn.ReceivedAt,
                CompletedAt = orderReturn.CompletedAt,

                Items = orderReturn.Items
                    .Select(item => new OrderReturnItemDto
                    {
                        Id = item.Id,
                        ReturnId = item.ReturnId,
                        OrderItemId = item.OrderItemId,
                        Quantity = item.Quantity,
                        ConditionNotes = item.ConditionNotes
                    })
                    .ToList()
            };
        }

        public async Task<(IReadOnlyList<OrderReturnDto> Items, int TotalCount)> GetPagedAsync(OrderReturnFilterParameters filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            if (filters.FromDate.HasValue &&
                filters.ToDate.HasValue &&
                filters.FromDate.Value.Date > filters.ToDate.Value.Date)
            {
                throw new ArgumentException("FromDate cannot be later than ToDate.");

            }

            filters.PageNumber = Math.Max(1, filters.PageNumber);
            filters.PageSize = Math.Clamp(filters.PageSize, 1, 100);

            var result = await _unitOfWork.OrderReturns
                .GetPagedAsync(filters);

            var items = result.Items
                .Select(MapToDto)
                .ToList();

            return (items, result.TotalCount);
        }

    }
}
