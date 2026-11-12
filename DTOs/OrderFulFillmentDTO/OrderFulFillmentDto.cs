
using Veltrix.Enums;

namespace Veltrix.DTOs.OrderFulfillmentDTO
{
    public class OrderFulfillmentDto
    {
        public int Id { get; set; }
        public string FulfillmentNumber { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public FulfillmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public IReadOnlyList<FulfillmentItemDto> Items { get; set; }
            = Array.Empty<FulfillmentItemDto>();
    }
}
