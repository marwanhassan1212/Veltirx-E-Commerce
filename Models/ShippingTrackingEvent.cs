namespace Veltrix.Models
{
    public class ShipmentTrackingEvent
    {
        public long Id { get; set; }

        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }

        public DateTime OccurredAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public Shipment Shipment { get; set; } = null!;
        public int ShipmentId { get; set; }
    }
}