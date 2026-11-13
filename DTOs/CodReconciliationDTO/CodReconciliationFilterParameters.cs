
using Veltrix.Enums;

namespace Veltrix.Repositories.CodReconciliation
{
    public class CodReconciliationFilterParameters
    {
        public string? ReconciliationNumber { get; set; }
        public CodReconciliationStatus? Status { get; set; }
        public string? ReconciledByUserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
