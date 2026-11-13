
using Veltrix.DTOs.RefundDTO;
using Veltrix.Repositories.Refund;
using Veltrix.UnitOfWork;
using RefundModel = Veltrix.Models.Refund;

namespace Veltrix.Services.Refund
{
    public class RefundService : IRefundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RefundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RefundDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid refund ID.");

            var refund = await _unitOfWork.Refunds.GetByIdAsync(id);

            return refund == null ? null : MapToDto(refund);
        }

        public async Task<RefundDto?> GetByRefundNumberAsync(
            string refundNumber)
        {
            if (string.IsNullOrWhiteSpace(refundNumber))
                throw new ArgumentException("Refund number is required.");

            var refund = await _unitOfWork.Refunds
                .GetByRefundNumberAsync(refundNumber.Trim());

            return refund == null ? null : MapToDto(refund);
        }

        public async Task<IReadOnlyList<RefundDto>> GetByReturnIdAsync(
            int returnId)
        {
            if (returnId <= 0)
                throw new ArgumentException("Invalid return ID.");

            var refunds = await _unitOfWork.Refunds
                .GetByReturnIdAsync(returnId);

            return refunds.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<RefundDto>> GetAllAsync()
        {
            var refunds = await _unitOfWork.Refunds.GetAllAsync();

            return refunds.Select(MapToDto).ToList();
        }

        private static RefundDto MapToDto(RefundModel refund)
        {
            return new RefundDto
            {
                Id = refund.Id,
                RefundNumber = refund.RefundNumber,
                ReturnId = refund.ReturnId,
                Amount = refund.Amount,
                Status = refund.Status,
                Reason = refund.Reason,
                Notes = refund.Notes,
                CreatedAt = refund.CreatedAt,
                ProcessedAt = refund.ProcessedAt,
                ProcessedByUserId = refund.ProcessedByUserId
            };
        }

        public async Task<(IReadOnlyList<RefundDto> Items, int TotalCount)> GetPagedAsync(RefundFilterParameters filters)
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
            var result = await _unitOfWork.Refunds.GetPagedAsync(filters);
            var items = result.Items.Select(MapToDto).ToList();
            return (items, result.TotalCount);
        }

    }
}
