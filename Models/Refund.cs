using Veltrix.Enums;

namespace Veltrix.Models
{
    public class Refund
    {
        public int Id { get; set; }

        public string RefundNumber { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public RefundStatus Status { get; set; }

        public string? Reason { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }

        public OrderReturn Return { get; set; } = null!;
        public int ReturnId { get; set; }

        public ApplicationUser? ProcessedByUser { get; set; }
        public string? ProcessedByUserId { get; set; }
    }
}