
using Veltrix.Enums;

namespace Veltrix.Repositories.OrderReturn
{
    public class OrderReturnFilterParameters
    {
        public string? ReturnNumber { get; set; }
        public int? OrderId { get; set; }
        public ReturnStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
