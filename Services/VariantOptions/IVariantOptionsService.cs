using Veltrix.DTOs.VariantOptionsDTO;

namespace Veltrix.Services.VariantOptions
{
    public interface IVariantOptionsService
    {
        Task<IReadOnlyList<VariantOptionsDto>> GetByProductIdAsync(int productId);
        Task<VariantOptionsDto> CreateAsync(CreateVariantOptionsDto dto);
        Task<VariantOptionsDto?> UpdateAsync(int id, UpdateVariantOptionsDto dto);
        Task<bool> DeleteAsync(int id);
    }
}