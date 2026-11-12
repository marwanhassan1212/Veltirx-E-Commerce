using Veltrix.DTOs.ShipmentTrackingEventDTO;

namespace Veltrix.Services.ShipmentTrackingEvent
{
    public interface IShipmentTrackingEventService
    {
        Task<ShipmentTrackingEventDto?> GetByIdAsync(long id);
        Task<IReadOnlyList<ShipmentTrackingEventDto>> GetByShipmentIdAsync(int shipmentId);
        Task<IReadOnlyList<ShipmentTrackingEventDto>> GetAllAsync();
    }
}
