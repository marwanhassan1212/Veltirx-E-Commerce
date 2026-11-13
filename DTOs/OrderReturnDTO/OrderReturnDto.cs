
using Veltrix.Enums;

namespace Veltrix.DTOs.OrderReturnDTO
{
    public class OrderReturnDto
    {
        public int Id { get; set; }
        public string ReturnNumber { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public ReturnStatus Status { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? CustomerNotes { get; set; }
        public string? AdminNotes { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public IReadOnlyList<OrderReturnItemDto> Items { get; set; }
            = Array.Empty<OrderReturnItemDto>();
    }
}
