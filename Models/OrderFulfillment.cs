using Veltrix.Enums;

namespace Veltrix.Models
{
    public class OrderFulfillment
    {
        public int Id { get; set; }

        public string FulfillmentNumber { get; set; } = string.Empty;

        public FulfillmentStatus Status { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Order Order { get; set; } = null!;
        public int OrderId { get; set; }

        public ICollection<FulfillmentItem> Items { get; set; }
            = new List<FulfillmentItem>();
    }
}
