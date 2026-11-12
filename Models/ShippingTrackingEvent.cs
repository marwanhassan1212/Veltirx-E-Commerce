using Veltrix.Enums;
namespace Veltrix.Models
{
    public class ShipmentTrackingEvent
    {
        public long Id { get; set; }
        public ShipmentTrackingStatus Status { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime OccurredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public Shipment Shipment { get; set; } = null!;
        public int ShipmentId { get; set; }
    }
}