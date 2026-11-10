
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.Shipment
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly AppDbContext _db;
        public ShipmentRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.Shipment shipment)
        {
            await _db.Shipments.AddAsync(shipment);
        }

        public async Task<IReadOnlyList<Models.Shipment>> GetAllAsync()
        {
            return await _db.Shipments.AsNoTracking().OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Models.Shipment?> GetByIdAsync(int id)
        {
            return await _db.Shipments
               .Include(x => x.TrackingEvents)
               .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.Shipment>> GetByOrderIdAsync(int orderId)
        {
            return await _db.Shipments
                .Where(x => x.OrderId == orderId)
                .AsNoTracking()
                .Include(x => x.TrackingEvents)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Models.Shipment?> GetByTrackingNumberAsync(string trackingNumber)
        {
            return await _db.Shipments
             .Include(x => x.TrackingEvents)
             .FirstOrDefaultAsync(
                 x => x.TrackingNumber == trackingNumber);
        }

        public void Update(Models.Shipment shipment)
        {
            _db.Shipments.Update(shipment);
        }
    }
}
