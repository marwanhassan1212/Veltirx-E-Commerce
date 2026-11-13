
using Veltrix.Enums;

namespace Veltrix.DTOs.RefundDTO
{
    public class RefundDto
    {
        public int Id { get; set; }
        public string RefundNumber { get; set; } = string.Empty;
        public int ReturnId { get; set; }
        public decimal Amount { get; set; }
        public RefundStatus Status { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ProcessedByUserId { get; set; }
    }
}
