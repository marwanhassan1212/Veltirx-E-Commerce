
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.ShipmentTrackingEvent
{
    public class ShipmentTrackingEventRepository : IShipmentTrackingEventRepository
    {
        private readonly AppDbContext _db;
        public ShipmentTrackingEventRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.ShipmentTrackingEvent trackingEvent)
        {
            await _db.ShipmentTrackingEvents.AddAsync(trackingEvent);
        }

        public void Delete(Models.ShipmentTrackingEvent trackingEvent)
        {
            _db.ShipmentTrackingEvents.Remove(trackingEvent);
        }

        public async Task<IReadOnlyList<Models.ShipmentTrackingEvent>> GetAllAsync()
        {
            return await _db.ShipmentTrackingEvents.AsNoTracking()
                .OrderByDescending(x => x.OccurredAt)
                .ToListAsync();
        }

        public async Task<Models.ShipmentTrackingEvent?> GetByIdAsync(long id)
        {
            return await _db.ShipmentTrackingEvents
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.ShipmentTrackingEvent>> GetByShipmentIdAsync(int shipmentId)
        {
            return await _db.ShipmentTrackingEvents
               .Where(x => x.ShipmentId == shipmentId)
               .AsNoTracking()
               .OrderByDescending(x => x.OccurredAt)
               .ThenByDescending(x => x.Id)
               .ToListAsync();
        }

        public void Update(Models.ShipmentTrackingEvent trackingEvent)
        {
            _db.ShipmentTrackingEvents.Update(trackingEvent);
        }
    }
}
