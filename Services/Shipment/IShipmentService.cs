using Veltrix.DTOs.ShipmentDTO;

namespace Veltrix.Services.Shipment
{
    public interface IShipmentService
    {
        Task<ShipmentDto?> GetByIdAsync(int id);
        Task<ShipmentDto?> GetByTrackingNumberAsync(string trackingNumber);
        Task<IReadOnlyList<ShipmentDto?>> GetByOrderIdAsync(int orderId);
    }
}
