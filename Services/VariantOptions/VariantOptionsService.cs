using Veltrix.DTOs.VariantOptionsDTO;
using Veltrix.UnitOfWork;

namespace Veltrix.Services.VariantOptions
{
    public class VariantOptionsService : IVariantOptionsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VariantOptionsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private static VariantOptionsDto MapToDto(Models.VariantOptions option)
        {
            return new VariantOptionsDto
            {
                Id = option.Id,
                Name = option.Name,
                DisplayOrder = option.DisplayOrder,
                ProductId = option.ProductId,

                Values = option.Values
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new VariantOptionValueDto
                    {
                        Id = x.Id,
                        Value = x.Value,
                        DisplayOrder = x.DisplayOrder
                    })
                    .ToList()
            };
        }

        public async Task<IReadOnlyList<VariantOptionsDto>> GetByProductIdAsync(int productId)
        {
            if (productId <= 0)
                return Array.Empty<VariantOptionsDto>();

            var options = await _unitOfWork.VariantOptions
                .GetByProductIdAsync(productId);

            return options.Select(MapToDto).ToList();
        }

        public async Task<VariantOptionsDto> CreateAsync(CreateVariantOptionsDto dto)
        {
            var product = await _unitOfWork.Product
                .GetByIdAsync(dto.ProductId);

            if (product == null)
                throw new InvalidOperationException(
                    "Product does not exist.");

            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException(
                    "Option group name is required.");

            var existingOptions = await _unitOfWork.VariantOptions
                .GetByProductIdAsync(dto.ProductId);

            if (existingOptions.Any(x =>
                string.Equals(
                    x.Name.Trim(),
                    name,
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "An option group with this name already exists.");
            }

            var option = new Models.VariantOptions
            {
                Name = name,
                DisplayOrder = dto.DisplayOrder,
                ProductId = dto.ProductId
            };

            await _unitOfWork.VariantOptions.AddAsync(option);

            await _unitOfWork.SaveChangesAsync();

            return MapToDto(option);
        }

        public async Task<VariantOptionsDto?> UpdateAsync(int id, UpdateVariantOptionsDto dto)
        {
            if (id <= 0)
                return null;

            var option = await _unitOfWork.VariantOptions
                .GetByIdAsync(id);

            if (option == null)
                return null;

            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException(
                    "Option group name is required.");

            var existingOptions = await _unitOfWork.VariantOptions
                .GetByProductIdAsync(option.ProductId);

            if (existingOptions.Any(x =>
                x.Id != id &&
                string.Equals(
                    x.Name.Trim(),
                    name,
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "An option group with this name already exists.");
            }

            option.Name = name;
            option.DisplayOrder = dto.DisplayOrder;

            await _unitOfWork.SaveChangesAsync();

            return MapToDto(option);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                return false;

            var option = await _unitOfWork.VariantOptions
                .GetByIdAsync(id);

            if (option == null)
                return false;

            if (option.Values.Any())
                throw new InvalidOperationException(
                    "Remove all option values before deleting this group.");

            _unitOfWork.VariantOptions.Delete(option);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}