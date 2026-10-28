namespace Veltrix.Repositories.Notification
{
    public interface INotificationRepository
    {
        Task<Models.Notification?> GetByIdAsync(int id);
        Task<IReadOnlyList<Models.Notification>> GetByUserIdAsync(string userId);
        Task<IReadOnlyList<Models.Notification>> GetUnreadByUserIdAsync(string userId);
        Task AddAsync(Models.Notification notification);
        void Update(Models.Notification notification);
    }
}
