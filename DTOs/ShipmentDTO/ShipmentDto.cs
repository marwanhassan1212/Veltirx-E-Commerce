using Veltrix.Enums;

namespace Veltrix.DTOs.ShipmentDTO
{
    public class ShipmentDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public string? CarrierName { get; set; }
        public ShipmentStatus status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? EstimatedDeliveryAt { get; set; }
        public string? Notes { get; set; }
    }
}
