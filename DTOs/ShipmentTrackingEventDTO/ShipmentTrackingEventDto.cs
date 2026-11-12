
namespace Veltrix.DTOs.ShipmentTrackingEventDTO
{
    public class ShipmentTrackingEventDto
    {
        public long Id { get; set; }
        public int ShipmentId { get; set; }
        public ShipmentTrackingStatus Status { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime OccurredAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
