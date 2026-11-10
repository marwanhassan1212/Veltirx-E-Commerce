namespace Veltrix.Repositories.ShipmentTrackingEvent
{
    public interface IShipmentTrackingEventRepository
    {
        Task<Models.ShipmentTrackingEvent?> GetByIdAsync(long id);

        Task<IReadOnlyList<Models.ShipmentTrackingEvent>> GetByShipmentIdAsync(int shipmentId);

        Task<IReadOnlyList<Models.ShipmentTrackingEvent>> GetAllAsync();
        Task AddAsync(Models.ShipmentTrackingEvent trackingEvent);
        void Update(Models.ShipmentTrackingEvent trackingEvent);
        void Delete(Models.ShipmentTrackingEvent trackingEvent);
}
}
