
using Veltrix.Enums;

namespace Veltrix.Repositories.Refund
{
    public class RefundFilterParameters
    {
        public string? RefundNumber { get; set; }
        public int? ReturnId { get; set; }
        public RefundStatus? Status { get; set; }
        public string? ProcessedByUserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
