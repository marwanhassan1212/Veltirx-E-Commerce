
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.UserAddress
{
    public class UserAddressRepository : IUserAddressRepository
    {
        private readonly AppDbContext _db;
        public UserAddressRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.UserAddress userAddress)
        {
            await _db.UserAddresses.AddAsync(userAddress);
        }

        public void Delete(Models.UserAddress userAddress)
        {
            _db.UserAddresses.Remove(userAddress);
        }

        public async Task<Models.UserAddress?> GetByIdAsync(int id)
        {
            return await _db.UserAddresses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Models.UserAddress>> GetByUserIdAsync(string userId)
        {
            return await _db.UserAddresses.Where(x => x.UserId == userId)
                .AsNoTracking().OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<Models.UserAddress?> GetDefaultByUserIdAsync(string userId)
        {
            return await _db.UserAddresses
                .AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId && x.IsDefault);
        }

        public void Update(Models.UserAddress userAddress)
        {
            _db.UserAddresses.Update(userAddress);
        }
    }
}
