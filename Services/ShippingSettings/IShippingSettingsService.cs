
using Veltrix.DTOs.ShippingSettingsDTO;

namespace Veltrix.Services.ShippingSettings
{
    public interface IShippingSettingsService
    {
        Task<ShippingSettingsDto?> GetByIdAsync(int id);
        Task<ShippingSettingsDto?> GetActiveAsync();
        Task<IReadOnlyList<ShippingSettingsDto>> GetAllAsync();
        Task<ShippingSettingsDto> UpdateShippingFeeAsync(decimal shippingFee, string? updatedByUserId);


    }
}
