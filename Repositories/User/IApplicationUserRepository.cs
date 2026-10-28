using Veltrix.Models;

namespace Veltrix.Repositories.User
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string userId);
        Task<IReadOnlyList<ApplicationUser>> GetAllAsync();
        Task<IReadOnlyList<ApplicationUser>> GetActiveUsersAsync();
        void Update(ApplicationUser user);
    }
}
