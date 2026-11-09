using Veltrix.Enums;

namespace Veltrix.Models
{
    public class Shipment
    {
        public int Id { get; set; }

        public string TrackingNumber { get; set; } = string.Empty;
        public string? CarrierName { get; set; }

        public ShipmentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? EstimatedDeliveryAt { get; set; }

        public string? Notes { get; set; }

        public Order Order { get; set; } = null!;
        public int OrderId { get; set; }

        public ICollection<ShipmentTrackingEvent> TrackingEvents { get; set; }
            = new List<ShipmentTrackingEvent>();
    }
}