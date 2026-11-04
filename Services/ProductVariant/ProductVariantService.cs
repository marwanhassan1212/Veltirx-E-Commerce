
using Veltrix.DTOs.ProductVariantDTO;
using Veltrix.Models;
using Veltrix.UnitOfWork;

namespace Veltrix.Services.ProductVariant
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductVariantService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // Mapping
        private static ProductVariantDto MapToDto(Models.ProductVariant variant)
        {
            return new ProductVariantDto
            {
                Id = variant.Id,
                SKU = variant.SKU,
                Price = variant.Price,
                IsDefault = variant.IsDefault,
                IsActive = variant.IsActive,
                ProductId = variant.ProductId,
                CreatedAt = variant.CreatedAt,
                UpdatedAt = variant.UpdatedAt,

                VariantOptionValueIds = variant.VariantOptions?
                    .Where(x => x.VariantOptionValue != null)
                    .Select(x => x.VariantOptionValueId)
                    .ToList() ?? new List<int>(),

                SelectedOptions = variant.VariantOptions?
                    .Where(x =>
                        x.VariantOptionValue != null &&
                        x.VariantOptionValue.VariantOption != null)
                    .Select(x => new ProductVariantOptionDto
                    {
                        OptionName = x.VariantOptionValue.VariantOption.Name,
                        Value = x.VariantOptionValue.Value
                    })
                    .OrderBy(x => x.OptionName)
                    .ToList() ?? new List<ProductVariantOptionDto>()
            };
        }


        // Get Variant By ID

        public async Task<ProductVariantDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            var variant =
                await unitOfWork.productVariant.GetByIdAsync(id);

            if (variant == null)
                return null;

            return MapToDto(variant);
        }

        // Get All Variants By Product ID

        public async Task<IReadOnlyList<ProductVariantDto>>
            GetByProductIdAsync(int productId)
        {
            if (productId <= 0)
                return Array.Empty<ProductVariantDto>();

            var variants =
                await unitOfWork.productVariant
                    .GetByProductIdAsync(productId);

            return variants
                .Select(MapToDto)
                .ToList();
        }

        // Create Variant
        public async Task<ProductVariantDto> CreateAsync(CreateProductVariantDto dto)
        {
            // Check Product
            var product =
                await unitOfWork.Product.GetByIdAsync(dto.ProductId);

            if (product == null)
                throw new InvalidOperationException(
                    "Product does not exist.");

            // Get Existing Variants
            var existingVariants =
                await unitOfWork.productVariant
                    .GetByProductIdAsync(dto.ProductId);

            // First Variant Becomes Default
            bool isFirstVariant = !existingVariants.Any();

            // Check SKU
            var sku = dto.SKU.Trim();

            var existingSKU =
                await unitOfWork.productVariant
                    .GetBySkuAsync(sku);

            if (existingSKU != null)
                throw new InvalidOperationException(
                    "SKU already exists.");

            // Create Variant
            var variant = new Models.ProductVariant
            {
                SKU = sku,
                Price = dto.Price,
                ProductId = dto.ProductId,
                IsDefault = isFirstVariant,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.productVariant.AddAsync(variant);

            await unitOfWork.SaveChangesAsync();

            return MapToDto(variant);
        }

        // Update Variant
        public async Task<ProductVariantDto?> UpdateAsync(int id, UpdateProductVariantDto dto)
        {
            var variant =
                await unitOfWork.productVariant.GetByIdAsync(id);

            if (variant == null)
                return null;

            // Check SKU
            var sku = dto.SKU.Trim();

            var existingSKU =
                await unitOfWork.productVariant
                    .GetBySkuAsync(sku);

            if (existingSKU != null && existingSKU.Id != id)
                throw new InvalidOperationException(
                    "SKU already exists.");

            // Update Properties
            variant.SKU = sku;
            variant.Price = dto.Price;
            variant.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync();

            return MapToDto(variant);
        }

        // Set Default Variant
        public async Task<bool> SetDefaultAsync(int id)
        {
            var variant = await unitOfWork.productVariant.GetByIdAsync(id);

            if (variant == null)
                return false;

            if (!variant.IsActive)
                throw new InvalidOperationException(
                    "Cannot set an inactive variant as default.");

            if (variant.IsDefault)
                return true;

            var variants = await unitOfWork.productVariant
                .GetByProductIdForUpdateAsync(variant.ProductId);

            var targetVariant = variants.FirstOrDefault(x => x.Id == id);

            if (targetVariant == null)
                return false;

            var now = DateTime.UtcNow;

            await using var transaction =
                await unitOfWork.BeginTransactionAsync();

            try
            {
                // Step 1: Remove the current default
                foreach (var item in variants)
                {
                    if (item.IsDefault)
                    {
                        item.IsDefault = false;
                        item.UpdatedAt = now;
                    }
                }

                await unitOfWork.SaveChangesAsync();

                // Step 2: Set the new default
                targetVariant.IsDefault = true;
                targetVariant.UpdatedAt = now;

                await unitOfWork.SaveChangesAsync();

                // Step 3: Commit transaction
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Activate Variant

        public async Task<bool> ActivateAsync(int id)
        {
            var variant =
                await unitOfWork.productVariant.GetByIdAsync(id);

            if (variant == null)
                return false;

            if (variant.IsActive)
                return true;

            variant.IsActive = true;
            variant.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync();

            return true;
        }


        // Deactivate Variant
     
        public async Task<bool> DeactivateAsync(int id)
        {
            var variant =
                await unitOfWork.productVariant.GetByIdAsync(id);

            if (variant == null)
                return false;

            if (!variant.IsActive)
                return true;

            // Default Variant Must Remain Active
            if (variant.IsDefault)
                throw new InvalidOperationException(
                    "Cannot deactivate the default variant. " +
                    "Set another variant as default first.");

            variant.IsActive = false;
            variant.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync();

            return true;
        }

      
        // Delete Variant
        public async Task<bool> DeleteAsync(int id)
        {
            var variant =
                await unitOfWork.productVariant.GetByIdAsync(id);

            if (variant == null)
                return false;

            // Cannot Delete Default Variant
            if (variant.IsDefault)
                throw new InvalidOperationException(
                    "Cannot delete the default variant.");

            // Cannot Delete Variant Linked To Orders
            if (variant.OrderItems.Any())
                throw new InvalidOperationException(
                    "Cannot delete a variant that is linked to previous orders.");

            // Cannot Delete Variant In Shopping Carts
            if (variant.CartItems.Any())
                throw new InvalidOperationException(
                    "Cannot delete a variant that exists in shopping carts.");

            // Cannot Delete Variant With Inventory
            if (variant.Inventory != null)
                throw new InvalidOperationException(
                    "Cannot delete a variant that has an inventory record.");

            // Remove Related Option Selections
            await unitOfWork.productVariant.RemoveOptionsNotInAsync(
                variant.Id,
                Array.Empty<int>());

            // Delete Variant
            unitOfWork.productVariant.Delete(variant);

            await unitOfWork.SaveChangesAsync();

            return true;
        }

        // Validate Selected Option Values
        private async Task<IReadOnlyList<Models.VariantOptionValue>> ValidateSelectedOptionsAsync(
                int productId, SetVariantOptionsDto dto)
        {
            if (dto.VariantOptionValueIds == null ||
                dto.VariantOptionValueIds.Count == 0)
            {
                throw new InvalidOperationException(
                    "At least one option value must be selected.");
            }

            // Prevent Duplicate IDs
            var selectedIds =
                dto.VariantOptionValueIds.Distinct().ToList();

            if (selectedIds.Count != dto.VariantOptionValueIds.Count)
                throw new InvalidOperationException(
                    "Duplicate option values are not allowed.");

            // Get Selected Values From Database
            var values =
                await unitOfWork.VariantOptionValues
                    .GetByIdsAsync(selectedIds);

            // Ensure All IDs Exist
            if (values.Count != selectedIds.Count)
                throw new InvalidOperationException(
                    "One or more selected option values do not exist.");

            // Ensure All Values Belong To The Same Product
            if (values.Any(x =>
                x.VariantOption.ProductId != productId))
            {
                throw new InvalidOperationException(
                    "Selected options must belong to the same product.");
            }

            // Prevent Multiple Values From The Same Group
            bool hasDuplicateGroups = values
                .GroupBy(x => x.VariantOptionId)
                .Any(group => group.Count() > 1);

            if (hasDuplicateGroups)
                throw new InvalidOperationException(
                    "Only one value can be selected from each option group.");

            // Get All Option Groups Of The Product
            var optionGroupIds =
                await unitOfWork.VariantOptionValues
                    .GetOptionGroupIdsByProductIdAsync(productId);

            // Get Selected Option Groups
            var selectedGroupIds = values
                .Select(x => x.VariantOptionId)
                .Distinct()
                .ToHashSet();

            // Every Group Must Have Exactly One Selected Value
            if (optionGroupIds.Count != selectedGroupIds.Count ||
                !optionGroupIds.All(
                    groupId => selectedGroupIds.Contains(groupId)))
            {
                throw new InvalidOperationException(
                    "You must select exactly one value from every option group.");
            }

            return values;
        }


        // Set Variant Options   
        public async Task<bool> SetOptionsAsync(
            int variantId,
            SetVariantOptionsDto dto)
        {
            var variant =
                await unitOfWork.productVariant.GetByIdAsync(variantId);

            if (variant == null)
                return false;

            // Cannot Change Options Linked To Previous Orders
            if (variant.OrderItems.Any())
                throw new InvalidOperationException(
                    "Cannot assign options to a variant linked to previous orders.");

            // Cannot Change Options In Shopping Carts
            if (variant.CartItems.Any())
                throw new InvalidOperationException(
                    "Cannot assign options to a variant currently in shopping carts.");

            // Prevent Assigning Options More Than Once
            if (variant.VariantOptions.Any())
                throw new InvalidOperationException(
                    "Options have already been assigned to this variant. Use UpdateOptions instead.");

            // Validate All Selected Options
            var values =
                await ValidateSelectedOptionsAsync(
                    variant.ProductId,
                    dto);

            // Add Option Selections
            foreach (var value in values)
            {
                await unitOfWork.productVariant.AddOptionAsync(
                    new Models.ProductVariantOption
                    {
                        ProductVariantId = variant.Id,
                        VariantOptionValueId = value.Id
                    });
            }

            await unitOfWork.SaveChangesAsync();

            return true;
        }

   
        // Update Variant Options
        public async Task<bool> UpdateOptionsAsync(
            int variantId,
            SetVariantOptionsDto dto)
        {
            var variant =
                await unitOfWork.productVariant.GetByIdAsync(variantId);

            if (variant == null)
                return false;

            // Cannot Change Options Linked To Previous Orders
            if (variant.OrderItems.Any())
                throw new InvalidOperationException(
                    "Cannot change options of a variant linked to previous orders.");

            // Cannot Change Options In Shopping Carts
            if (variant.CartItems.Any())
                throw new InvalidOperationException(
                    "Cannot change options of a variant currently in shopping carts.");

            // Validate Selected Options
            var values =
                await ValidateSelectedOptionsAsync(
                    variant.ProductId,
                    dto);

            // Get Existing Selected Values
            var existingValueIds = variant.VariantOptions
                .Select(x => x.VariantOptionValueId)
                .ToHashSet();

            var selectedValueIds = values
                .Select(x => x.Id)
                .ToHashSet();

            // Remove Values That Are No Longer Selected
            await unitOfWork.productVariant.RemoveOptionsNotInAsync(
                variantId,
                selectedValueIds);

            // Add Only New Values
            foreach (var value in values)
            {
                if (existingValueIds.Contains(value.Id))
                    continue;

                await unitOfWork.productVariant.AddOptionAsync(
                    new Models.ProductVariantOption
                    {
                        ProductVariantId = variant.Id,
                        VariantOptionValueId = value.Id
                    });
            }

            await unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Models.VariantOptions>> GetOptionGroupsWithValuesAsync(int productId)
        {
            if (productId <= 0)
                return Array.Empty<Models.VariantOptions>();

            return await unitOfWork.VariantOptionValues
                .GetGroupsWithValuesByProductIdAsync(productId);
        }
    }
}

