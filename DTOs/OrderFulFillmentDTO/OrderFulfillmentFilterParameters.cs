
using Veltrix.Enums;

namespace Veltrix.Repositories.OrderFulfillment
{
    public class OrderFulfillmentFilterParameters
    {
        public string? FulfillmentNumber { get; set; }
        public int? OrderId { get; set; }
        public FulfillmentStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
