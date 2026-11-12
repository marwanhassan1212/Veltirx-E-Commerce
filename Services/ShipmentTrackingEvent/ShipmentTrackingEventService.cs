
using Veltrix.DTOs.ShipmentTrackingEventDTO;
using Veltrix.UnitOfWork;

using TrackingEventModel = Veltrix.Models.ShipmentTrackingEvent;

namespace Veltrix.Services.ShipmentTrackingEvent
{
    public class ShipmentTrackingEventService
        : IShipmentTrackingEventService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShipmentTrackingEventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ShipmentTrackingEventDto?> GetByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "Invalid tracking event ID.");

            var trackingEvent = await _unitOfWork.ShipmentTrackingEvents.GetByIdAsync(id);


            if (trackingEvent == null)
                return null;

            return MapToDto(trackingEvent);
        }

        public async Task<IReadOnlyList<ShipmentTrackingEventDto>> GetByShipmentIdAsync(int shipmentId)

        {
            if (shipmentId <= 0)
                throw new ArgumentException(
                    "Invalid shipment ID.");

            var events = await _unitOfWork
                .ShipmentTrackingEvents
                .GetByShipmentIdAsync(shipmentId);

            return events.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<ShipmentTrackingEventDto>> GetAllAsync()
        {
            var events = await _unitOfWork
                .ShipmentTrackingEvents.GetAllAsync();

            return events.Select(MapToDto).ToList();
        }

        private static ShipmentTrackingEventDto MapToDto(TrackingEventModel trackingEvent)

        {
            return new ShipmentTrackingEventDto
            {
                Id = trackingEvent.Id,
                ShipmentId = trackingEvent.ShipmentId,
                Status = trackingEvent.Status,
                Description = trackingEvent.Description,
                Location = trackingEvent.Location,
                OccurredAt = trackingEvent.OccurredAt,
                CreatedAt = trackingEvent.CreatedAt
            };
        }
    }
}
