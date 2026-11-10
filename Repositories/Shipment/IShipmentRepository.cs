namespace Veltrix.Repositories.Shipment
{
    public interface IShipmentRepository
    {
        Task<Models.Shipment?> GetByIdAsync(int id);

        Task<Models.Shipment?> GetByTrackingNumberAsync(string trackingNumber);

        Task<IReadOnlyList<Models.Shipment>> GetByOrderIdAsync(int orderId);

        Task<IReadOnlyList<Models.Shipment>> GetAllAsync();

        Task AddAsync(Models.Shipment shipment);

        void Update(Models.Shipment shipment);
    }
}
