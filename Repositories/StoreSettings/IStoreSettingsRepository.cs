namespace Veltrix.Repositories.StoreSettings
{
    public interface IStoreSettingsRepository
    {
        Task<Models.StoreSettings?> GetByIdAsync(int id);
        Task<Models.StoreSettings?> GetActiveAsync();
        Task<IReadOnlyList<Models.StoreSettings>> GetAllAsync();
        Task AddAsync(Models.StoreSettings settings);
        void Update(Models.StoreSettings settings);
    }
}
