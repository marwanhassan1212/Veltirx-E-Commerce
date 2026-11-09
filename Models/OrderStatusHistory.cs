using Veltrix.Enums;

namespace Veltrix.Models
{
    public class OrderStatusHistory
    {
        public long Id { get; set; }

        public OrderStatus? PreviousStatus { get; set; }
        public OrderStatus NewStatus { get; set; }

        public string? Note { get; set; }
        public DateTime ChangedAt { get; set; }




        public Order Order { get; set; } = null!;
        public int OrderId { get; set; }

        public ApplicationUser? ChangedByUser { get; set; }
        public string? ChangedByUserId { get; set; }
    }
}
