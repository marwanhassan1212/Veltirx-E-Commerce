using Microsoft.EntityFrameworkCore;
using Veltrix.Data;
using Veltrix.Models;

namespace Veltrix.Repositories.User
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly AppDbContext _db;
        public ApplicationUserRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IReadOnlyList<ApplicationUser>> GetActiveUsersAsync()
        {
            return await _db.Users.Where(x => x.IsActive)
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id).ToListAsync();
        }

        public async Task<IReadOnlyList<ApplicationUser>> GetAllAsync()
        {
            return await _db.Users
                .AsNoTracking().OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id).ToListAsync();
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return await _db.Users
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);    
        }

        public void Update(ApplicationUser user)
        {
            _db.Users.Update(user);
        }
    }
}
