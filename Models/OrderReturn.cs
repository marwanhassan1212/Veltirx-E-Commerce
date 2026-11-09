using Microsoft.CodeAnalysis;
using Veltrix.Enums;

namespace Veltrix.Models
{
    public class OrderReturn
    {
        public int Id { get; set; }

        public string ReturnNumber { get; set; } = string.Empty;

        public ReturnStatus Status { get; set; }

        public string Reason { get; set; } = string.Empty;
        public string? CustomerNotes { get; set; }
        public string? AdminNotes { get; set; }

        public DateTime RequestedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Order Order { get; set; } = null!;
        public int OrderId { get; set; }

        public ICollection<OrderReturnItem> Items { get; set; }
            = new List<OrderReturnItem>();

        public ICollection<Refund> Refunds { get; set; }
            = new List<Refund>();
    }
}