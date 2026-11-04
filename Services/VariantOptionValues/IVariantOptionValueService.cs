using Veltrix.DTOs.VariantOptionValueDTO;

namespace Veltrix.Services.VariantOptionValues
{
    public interface IVariantOptionValueService
    {
        Task<IReadOnlyList<VariantOptionValueDto>> GetByGroupIdAsync(int groupId);
        Task<VariantOptionValueDto> CreateAsync(CreateVariantOptionValueDto dto);
        Task<VariantOptionValueDto?> UpdateAsync(int id, UpdateVariantOptionValueDto dto);
        Task<bool> DeleteAsync(int id);
    }
}