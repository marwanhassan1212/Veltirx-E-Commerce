using Veltrix.Enums;
using Veltrix.Models;

namespace Veltrix.Repositories.Order
{
    public class OrderFilterParameters
    {
        public string? OrderNumber { get; set; }
        public string? UserId { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}