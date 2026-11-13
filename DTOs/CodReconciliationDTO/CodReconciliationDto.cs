
using Veltrix.Enums;

namespace Veltrix.DTOs.CodReconciliationDTO
{
    public class CodReconciliationDto
    {
        public int Id { get; set; }
        public string ReconciliationNumber { get; set; } = string.Empty;
        public decimal ExpectedAmount { get; set; }
        public decimal CollectedAmount { get; set; }
        public decimal DifferenceAmount { get; set; }
        public CodReconciliationStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReconciledAt { get; set; }
        public string? ReconciledByUserId { get; set; }

        public IReadOnlyList<CodReconciliationItemDto> Items { get; set; }
            = new List<CodReconciliationItemDto>();
    }
}
