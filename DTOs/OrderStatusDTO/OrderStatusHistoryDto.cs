using Veltrix.Enums;

namespace Veltrix.DTOs.OrderStatusDTO
{
    public class OrderStatusHistoryDto
    {
        public long Id { get; set; }
        public int OrderId { get; set; }
        public OrderStatus? PreviousStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public string? Note { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? ChangedByUserId { get; set; }
    }
}
