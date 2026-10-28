namespace Veltrix.Repositories.ShippingSettings
{
    public interface IShippingSettingsRepository
    {
        Task<Models.ShippingSettings?> GetByIdAsync(int id);
        Task<Models.ShippingSettings?> GetActiveAsync();
        Task<IReadOnlyList<Models.ShippingSettings>> GetAllAsync();
        Task AddAsync(Models.ShippingSettings settings);
        void Update(Models.ShippingSettings settings);
    }
}
