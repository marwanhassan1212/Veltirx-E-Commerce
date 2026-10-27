namespace Veltrix.Models
{
    public class OrderStatusHistory
    {
        public long Id { get; set; }

        public string? PreviousStatus { get; set; }
        public string NewStatus { get; set; } = string.Empty;

        public string? Note { get; set; }
        public DateTime ChangedAt { get; set; }




        public Order Order { get; set; } = null!;
        public int OrderId { get; set; }

        public ApplicationUser? ChangedByUser { get; set; }
        public string? ChangedByUserId { get; set; }
    }
}
