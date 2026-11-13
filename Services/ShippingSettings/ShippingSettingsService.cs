
using Veltrix.DTOs.ShippingSettingsDTO;
using Veltrix.UnitOfWork;
using ShippingSettingsModel = Veltrix.Models.ShippingSettings;

namespace Veltrix.Services.ShippingSettings
{
    public class ShippingSettingsService : IShippingSettingsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShippingSettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ShippingSettingsDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid shipping settings ID.");

            var settings = await _unitOfWork.ShippingSettings.GetByIdAsync(id);
            return settings == null ? null : MapToDto(settings);
        }

        public async Task<ShippingSettingsDto?> GetActiveAsync()
        {
            var settings = await _unitOfWork.ShippingSettings.GetActiveAsync();


            return settings == null ? null : MapToDto(settings);
        }

        public async Task<IReadOnlyList<ShippingSettingsDto>> GetAllAsync()
        {
            var settings = await _unitOfWork.ShippingSettings.GetAllAsync();


            return settings.Select(MapToDto).ToList();
        }

        public async Task<ShippingSettingsDto> UpdateShippingFeeAsync(
            decimal shippingFee,
            string? updatedByUserId)
        {
            if (shippingFee < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(shippingFee), "Shipping fee cannot be negative.");
            }
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var currentSettings = await _unitOfWork.ShippingSettings.GetActiveAsync();
                if (currentSettings != null)
                {
                    currentSettings.IsActive = false;

                    _unitOfWork.ShippingSettings.Update(currentSettings);
                }
                var newSettings = new Models.ShippingSettings
                {
                    ShippingFee = shippingFee,
                    IsActive = true,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedByUserId = updatedByUserId
                };
                await _unitOfWork.ShippingSettings.AddAsync(newSettings);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
                return MapToDto(newSettings);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        private static ShippingSettingsDto MapToDto(ShippingSettingsModel settings)

        {
            return new ShippingSettingsDto
            {
                Id = settings.Id,
                ShippingFee = settings.ShippingFee,
                IsActive = settings.IsActive,
                UpdatedAt = settings.UpdatedAt,
                UpdatedByUserId = settings.UpdatedByUserId
            };
        }
    }
}
