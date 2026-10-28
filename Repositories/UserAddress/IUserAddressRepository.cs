namespace Veltrix.Repositories.UserAddress
{
    public interface IUserAddressRepository
    {
        Task<Models.UserAddress?> GetByIdAsync(int id);
        Task<IReadOnlyList<Models.UserAddress>> GetByUserIdAsync(string userId);
        Task<Models.UserAddress?> GetDefaultByUserIdAsync(string userId);
        Task AddAsync(Models.UserAddress userAddress);
        void Update(Models.UserAddress userAddress);
        void Delete(Models.UserAddress userAddress);
    }
}
