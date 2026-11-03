using Veltrix.DTOs.CategoryDTO;
using Veltrix.Models;
using Veltrix.UnitOfWork;

namespace Veltrix.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private static CategoryDto MapToDTo(Models.Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                ImageUrl = category.ImageUrl,
                Description = category.Description,
                DisplayOrder = category.DisplayOrder,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                IsActive = category.IsActive,
                ParentCategoryId = category.ParentCategoryId,
            };
        }
        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var existingSlug = await _unitOfWork.Category.GetBySlugAsync(dto.Slug);
            if (existingSlug != null)
                throw new InvalidOperationException("Slug already exists");

            await ValidateParentCategoryAsync(dto.ParentCategoryId);

            var category = new Models.Category
            {
                Name = dto.Name,
                Slug = dto.Slug,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                DisplayOrder = dto.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                IsActive = dto.IsActive,
                ParentCategoryId = dto.ParentCategoryId,
            };

            await _unitOfWork.Category.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return MapToDTo(category);
        }

        public async Task<IReadOnlyList<CategoryDto>> GetActiveCategoriesAsync()
        {
            var categories = await _unitOfWork.Category.GetActiveCategoriesAsync();
            return categories.Select(MapToDTo).ToList();
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync()
        {
            var categories = await _unitOfWork.Category.GetAllAsync();
            return categories.Select(MapToDTo).ToList();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Category.GetByIdAsync(id);
            if (category == null)
                return null;
            return MapToDTo(category);
        }

        public async Task<CategoryDto?> GetBySlugAsync(string slug)
        {
            var category = await _unitOfWork.Category.GetBySlugAsync(slug);
            if (category == null)
                return null;
            return MapToDTo(category);
        }

        public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var existingCategory = await _unitOfWork.Category.GetByIdAsync(id);
            if (existingCategory == null)
                return null;

            var existingSlug = await _unitOfWork.Category.GetBySlugAsync(dto.Slug);
            if (existingSlug != null && existingSlug.Id != id)
                throw new InvalidOperationException("Slug already exists");

            await ValidateParentCategoryAsync(dto.ParentCategoryId , id);
            if (existingCategory.IsActive && !dto.IsActive)
            {
                await ValidateDeactivationAsync(id);
            }

            existingCategory.Name = dto.Name;
            existingCategory.Slug = dto.Slug;
            existingCategory.Description = dto.Description;
            existingCategory.ImageUrl = dto.ImageUrl;
            existingCategory.DisplayOrder = dto.DisplayOrder;
            existingCategory.IsActive = dto.IsActive;
            existingCategory.UpdatedAt = DateTime.UtcNow;
            existingCategory.ParentCategoryId = dto.ParentCategoryId;

            await _unitOfWork.SaveChangesAsync();
            return MapToDTo(existingCategory);
        }

        private async Task ValidateParentCategoryAsync(
         int? parentCategoryId,
         int? currentCategoryId = null)
        {
            if (!parentCategoryId.HasValue)
                return;

            var categories = await _unitOfWork.Category.GetAllAsync();

            var categoryLookup = categories.ToDictionary(x => x.Id);

            // Check parent existence
            if (!categoryLookup.TryGetValue(
                parentCategoryId.Value, out var parentCategory))
            {
                throw new InvalidOperationException(
                    "Parent category does not exist");
            }

            var visited = new HashSet<int>();

            int? currentParentId = parentCategoryId;

            while (currentParentId.HasValue)
            {
                // Prevent circular relationships
                if (currentCategoryId.HasValue &&
                    currentParentId.Value == currentCategoryId.Value)
                {
                    throw new InvalidOperationException(
                        "Circular category relationship is not allowed");
                }

                // Protect against existing circular relationships
                if (!visited.Add(currentParentId.Value))
                {
                    throw new InvalidOperationException(
                        "Existing category hierarchy contains a circular relationship");
                }

                // Check hierarchy integrity
                if (!categoryLookup.TryGetValue(
                    currentParentId.Value, out var currentParent))
                {
                    throw new InvalidOperationException(
                        "Invalid category hierarchy");
                }

                // Every parent and ancestor must be active
                if (!currentParent.IsActive)
                {
                    throw new InvalidOperationException(
                        "Cannot assign category under an inactive parent or ancestor");
                }

                currentParentId = currentParent.ParentCategoryId;
            }
        }
        private async Task ValidateDeactivationAsync(int categoryId)
        {
            var hasActiveSubCategories =
                await _unitOfWork.Category.HasActiveSubCategoriesAsync(categoryId);

            if (hasActiveSubCategories)
            {
                throw new InvalidOperationException(
                    "Cannot deactivate a category that has active subcategories");
            }

            var hasPublishedProducts =
                await _unitOfWork.Category.HasPublishedProductsAsync(categoryId);

            if (hasPublishedProducts)
            {
                throw new InvalidOperationException(
                    "Cannot deactivate a category that contains published products");
            }
        }
    }
}
