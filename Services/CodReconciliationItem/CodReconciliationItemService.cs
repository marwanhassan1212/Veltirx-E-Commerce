
using Veltrix.DTOs.CodReconciliationDTO;
using Veltrix.UnitOfWork;

namespace Veltrix.Services.CodReconciliationItem
{
    public class CodReconciliationItemService
        : ICodReconciliationItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CodReconciliationItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CodReconciliationItemDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            var item = await _unitOfWork.CodReconciliationItems
                .GetByIdAsync(id);

            if (item == null)
                return null;

            return MapToDto(item);
        }

        public async Task<IReadOnlyList<CodReconciliationItemDto>> GetByReconciliationIdAsync(int reconciliationId)
        {
            if (reconciliationId <= 0)
                return Array.Empty<CodReconciliationItemDto>();

            var items = await _unitOfWork.CodReconciliationItems
                .GetByReconciliationIdAsync(reconciliationId);

            return items.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<CodReconciliationItemDto>> GetByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
                return Array.Empty<CodReconciliationItemDto>();

            var items = await _unitOfWork.CodReconciliationItems
                .GetByOrderIdAsync(orderId);

            return items.Select(MapToDto).ToList();
        }

        private static CodReconciliationItemDto MapToDto(Models.CodReconciliationItem item)
        {
            return new CodReconciliationItemDto
            {
                Id = item.Id,
                OrderId = item.OrderId,
                ExpectedAmount = item.ExpectedAmount,
                CollectedAmount = item.CollectedAmount,
                DifferenceAmount = item.DifferenceAmount
            };
        }
    }
}
