namespace Veltrix.DTOs.OrderStatusDTO
{
    public class OrderStatusHistoryDto
    {
        public string? PreviousStatus { get; set; }
        public string NewStatus { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? ChangedByUserId { get; set; }
        public string? ChangedByUserName { get; set; }
    }
}
