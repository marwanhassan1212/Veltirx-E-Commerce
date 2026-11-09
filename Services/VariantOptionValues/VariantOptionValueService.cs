using Veltrix.DTOs.VariantOptionValueDTO;
using Veltrix.UnitOfWork;

namespace Veltrix.Services.VariantOptionValues
{
    public class VariantOptionValueService : IVariantOptionValueService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VariantOptionValueService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private static VariantOptionValueDto MapToDto(Models.VariantOptionValue value)
        {
            return new VariantOptionValueDto
            {
                Id = value.Id,
                Value = value.Value,
                DisplayOrder = value.DisplayOrder,
                VariantOptionId = value.VariantOptionId
            };
        }

        public async Task<IReadOnlyList<VariantOptionValueDto>> GetByGroupIdAsync(int groupId)
        {
            if (groupId <= 0)
                return Array.Empty<VariantOptionValueDto>();

            var group = await _unitOfWork.VariantOptions.GetByIdAsync(groupId);

            if (group == null)
                return Array.Empty<VariantOptionValueDto>();

            return group.Values
                .OrderBy(x => x.DisplayOrder)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<VariantOptionValueDto> CreateAsync(
    CreateVariantOptionValueDto dto)
        {
            var group = await _unitOfWork.VariantOptions
                .GetByIdAsync(dto.VariantOptionId);

            if (group == null)
                throw new InvalidOperationException(
                    "Option group does not exist.");

            var valueText = dto.Value.Trim();

            if (string.IsNullOrWhiteSpace(valueText))
                throw new InvalidOperationException(
                    "Option value is required.");

            if (group.Values.Any(x =>
                string.Equals(
                    x.Value.Trim(),
                    valueText,
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "This value already exists in the option group.");
            }

            var nextDisplayOrder = group.Values.Any()
                ? group.Values.Max(x => x.DisplayOrder) + 1
                : 1;

            var optionValue = new Models.VariantOptionValue
            {
                Value = valueText,
                DisplayOrder = nextDisplayOrder,
                VariantOptionId = dto.VariantOptionId
            };

            await _unitOfWork.VariantOptionValues.AddAsync(optionValue);

            await _unitOfWork.SaveChangesAsync();

            return MapToDto(optionValue);
        }

        public async Task<VariantOptionValueDto?> UpdateAsync(int id, UpdateVariantOptionValueDto dto)


        {
            if (id <= 0)
                return null;

            var optionValue = await _unitOfWork.VariantOptionValues
                .GetByIdAsync(id);

            if (optionValue == null)
                return null;

            var valueText = dto.Value.Trim();

            if (string.IsNullOrWhiteSpace(valueText))
                throw new InvalidOperationException(
                    "Option value is required.");

            var group = await _unitOfWork.VariantOptions
                .GetByIdAsync(optionValue.VariantOptionId);

            if (group == null)
                throw new InvalidOperationException(
                    "Option group does not exist.");

            if (group.Values.Any(x =>
                x.Id != id &&
                string.Equals(
                    x.Value.Trim(),
                    valueText,
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "This value already exists in the option group.");
            }

            var values = group.Values
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Id)
                .ToList();


            optionValue.Value = valueText;

            await _unitOfWork.SaveChangesAsync();

            return MapToDto(optionValue);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                return false;

            var optionValue =
                await _unitOfWork.VariantOptionValues.GetByIdAsync(id);

            if (optionValue == null)
                return false;

            if (optionValue.ProductVariantOptions.Any())
            {
                throw new InvalidOperationException(
                    "Cannot delete an option value that is assigned to product variants.");
            }

            // Get the option group with its values
            var group = await _unitOfWork.VariantOptions
                .GetByIdAsync(optionValue.VariantOptionId);

            if (group == null)
                throw new InvalidOperationException("Option group does not exist.");

            // Get remaining values in their current order
            var remainingValues = group.Values
                .Where(x => x.Id != id)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Id)
                .ToList();

            // Delete selected value
            _unitOfWork.VariantOptionValues.Delete(optionValue);

            // Re-sequence remaining values
            for (int i = 0; i < remainingValues.Count; i++)
            {
                remainingValues[i].DisplayOrder = i + 1;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}