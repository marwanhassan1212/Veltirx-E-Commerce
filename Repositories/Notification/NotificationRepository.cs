using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.Notification
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _db;
        public NotificationRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Models.Notification notification)
        {
            await _db.Notifications.AddAsync(notification);
        }

        public async Task<Models.Notification?> GetByIdAsync(int id)
        {
            return await _db.Notifications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.Notification>> GetByUserIdAsync(string userId)
        {
            return await _db.Notifications.Where(x => x.UserId == userId)
                .AsNoTracking().OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id).ToListAsync();
        }

        public async Task<IReadOnlyList<Models.Notification>> GetUnreadByUserIdAsync(string userId)
        {
            return await _db.Notifications.Where(x => x.IsRead == false && x.UserId == userId)
                .AsNoTracking().OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).ToListAsync();
        }

        public void Update(Models.Notification notification)
        {
            _db.Notifications.Update(notification);
        }
    }
}
