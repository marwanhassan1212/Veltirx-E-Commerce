
using Veltrix.DTOs.ShipmentDTO;
using Veltrix.UnitOfWork;

using ShipmentModel = Veltrix.Models.Shipment;

namespace Veltrix.Services.Shipment
{
    public class ShipmentService : IShipmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShipmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ShipmentDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid shipment ID.");

            var shipment = await _unitOfWork.Shipments.GetByIdAsync(id);

            if (shipment == null)
                return null;

            return MapToDto(shipment);
        }

        public async Task<ShipmentDto?> GetByTrackingNumberAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException(
                    "Tracking number is required.");

            var shipment = await _unitOfWork.Shipments
                .GetByTrackingNumberAsync(trackingNumber.Trim());

            if (shipment == null)
                return null;

            return MapToDto(shipment);
        }

        public async Task<IReadOnlyList<ShipmentDto?>> GetByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid order ID.");

            var shipments = await _unitOfWork.Shipments
                .GetByOrderIdAsync(orderId);

            return shipments
                .Select(MapToDto)
                .ToList();
        }

        private static ShipmentDto MapToDto(ShipmentModel shipment)
        {
            return new ShipmentDto
            {
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                TrackingNumber = shipment.TrackingNumber,
                CarrierName = shipment.CarrierName,
                status = shipment.Status,
                CreatedAt = shipment.CreatedAt,
                ShippedAt = shipment.ShippedAt,
                DeliveredAt = shipment.DeliveredAt,
                EstimatedDeliveryAt = shipment.EstimatedDeliveryAt,
                Notes = shipment.Notes
            };
        }
    }
}
