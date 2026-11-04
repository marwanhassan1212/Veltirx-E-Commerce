using Veltrix.DTOs.CategoryDTO;
using Veltrix.Models;
using Veltrix.UnitOfWork;
using Veltrix.Services.Images;

namespace Veltrix.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageStorageService _imageStorage;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            IUnitOfWork unitOfWork,
            IImageStorageService imageStorage,
            ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _imageStorage = imageStorage;
            _logger = logger;
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
                ParentCategoryId = category.ParentCategoryId
            };
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var existingSlug =
                await _unitOfWork.Category.GetBySlugAsync(dto.Slug);

            if (existingSlug != null)
                throw new InvalidOperationException("Slug already exists");

            await ValidateParentCategoryAsync(dto.ParentCategoryId);

            string? uploadedImageUrl = null;

            try
            {
                if (dto.ImageFile != null)
                {
                    uploadedImageUrl = await _imageStorage.SaveAsync(
                        dto.ImageFile,
                        "categories");
                }

                // Get categories in the same level.
                var categories = await _unitOfWork.Category.GetAllAsync();

                var affectedCategories = categories
                    .Where(c =>
                        c.ParentCategoryId == dto.ParentCategoryId &&
                        c.DisplayOrder >= dto.DisplayOrder)
                    .OrderByDescending(c => c.DisplayOrder)
                    .ToList();

                // Shift existing categories one position down.
                foreach (var existingCategory in affectedCategories)
                {
                    existingCategory.DisplayOrder++;
                }

                var category = new Models.Category
                {
                    Name = dto.Name,
                    Slug = dto.Slug,
                    Description = dto.Description,
                    ImageUrl = uploadedImageUrl,
                    DisplayOrder = dto.DisplayOrder,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = dto.IsActive,
                    ParentCategoryId = dto.ParentCategoryId
                };

                await _unitOfWork.Category.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                return MapToDTo(category);
            }
            catch
            {
                if (uploadedImageUrl != null)
                {
                    try
                    {
                        await _imageStorage.DeleteAsync(uploadedImageUrl);
                    }
                    catch (Exception cleanupException)
                    {
                        _logger.LogError(
                            cleanupException,
                            "Failed to clean up uploaded category image: {ImageUrl}",
                            uploadedImageUrl);
                    }
                }

                throw;
            }
        }

        public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var existingCategory =
                await _unitOfWork.Category.GetByIdAsync(id);

            if (existingCategory == null)
                return null;

            var existingSlug =
                await _unitOfWork.Category.GetBySlugAsync(dto.Slug);

            if (existingSlug != null && existingSlug.Id != id)
                throw new InvalidOperationException("Slug already exists");

            await ValidateParentCategoryAsync(dto.ParentCategoryId, id);

            if (existingCategory.IsActive && !dto.IsActive)
            {
                await ValidateDeactivationAsync(id);
            }

            var oldParentCategoryId = existingCategory.ParentCategoryId;
            var oldImageUrl = existingCategory.ImageUrl;

            string? newImageUrl = null;

            try
            {
                if (dto.ImageFile != null)
                {
                    newImageUrl = await _imageStorage.SaveAsync(
                        dto.ImageFile,
                        "categories");
                }

                // Get all categories with EF Core tracking.
                var categories = await _unitOfWork.Category.GetAllAsync();

                // If the category moves to another parent,
                // close the gap in its previous group.
                if (oldParentCategoryId != dto.ParentCategoryId)
                {
                    var oldSiblings = categories
                        .Where(c =>
                            c.ParentCategoryId == oldParentCategoryId &&
                            c.Id != id)
                        .OrderBy(c => c.DisplayOrder)
                        .ThenBy(c => c.Id)
                        .ToList();

                    for (int i = 0; i < oldSiblings.Count; i++)
                    {
                        oldSiblings[i].DisplayOrder = i + 1;
                    }
                }

                // Get categories in the destination group.
                var destinationSiblings = categories
                    .Where(c =>
                        c.ParentCategoryId == dto.ParentCategoryId &&
                        c.Id != id)
                    .OrderBy(c => c.DisplayOrder)
                    .ThenBy(c => c.Id)
                    .ToList();

                // Determine the requested position.
                // Position starts from 1.
                int targetIndex = Math.Clamp(
                    dto.DisplayOrder - 1,
                    0,
                    destinationSiblings.Count);

                // Insert the edited category at the requested position.
                destinationSiblings.Insert(targetIndex, existingCategory);

                // Reassign positions sequentially.
                for (int i = 0; i < destinationSiblings.Count; i++)
                {
                    destinationSiblings[i].DisplayOrder = i + 1;
                }

                // Update category properties.
                existingCategory.Name = dto.Name;
                existingCategory.Slug = dto.Slug;
                existingCategory.Description = dto.Description;
                existingCategory.IsActive = dto.IsActive;
                existingCategory.ParentCategoryId = dto.ParentCategoryId;
                existingCategory.UpdatedAt = DateTime.UtcNow;

                if (newImageUrl != null)
                {
                    existingCategory.ImageUrl = newImageUrl;
                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                if (newImageUrl != null)
                {
                    try
                    {
                        await _imageStorage.DeleteAsync(newImageUrl);
                    }
                    catch (Exception cleanupException)
                    {
                        _logger.LogError(
                            cleanupException,
                            "Failed to clean up new category image: {ImageUrl}",
                            newImageUrl);
                    }
                }

                throw;
            }

            // Delete old image only after successful database update.
            if (newImageUrl != null &&
                !string.IsNullOrWhiteSpace(oldImageUrl))
            {
                try
                {
                    await _imageStorage.DeleteAsync(oldImageUrl);
                }
                catch (Exception cleanupException)
                {
                    _logger.LogWarning(
                        cleanupException,
                        "Could not delete old category image: {ImageUrl}",
                        oldImageUrl);
                }
            }

            return MapToDTo(existingCategory);
        }

        public async Task<IReadOnlyList<CategoryDto>> GetActiveCategoriesAsync()
        {
            var categories =
                await _unitOfWork.Category.GetActiveCategoriesAsync();

            return categories.Select(MapToDTo).ToList();
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync()
        {
            return await _unitOfWork.Category.GetCategoriesForListAsync();
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
            var category =
                await _unitOfWork.Category.GetBySlugAsync(slug);

            if (category == null)
                return null;

            return MapToDTo(category);
        }

        private async Task ValidateParentCategoryAsync(
            int? parentCategoryId,
            int? currentCategoryId = null)
        {
            if (!parentCategoryId.HasValue)
                return;

            var categories = await _unitOfWork.Category.GetAllAsync();

            var categoryLookup = categories.ToDictionary(x => x.Id);

            if (!categoryLookup.TryGetValue(
                parentCategoryId.Value,
                out var parentCategory))
            {
                throw new InvalidOperationException(
                    "Parent category does not exist");
            }

            var visited = new HashSet<int>();

            int? currentParentId = parentCategoryId;

            while (currentParentId.HasValue)
            {
                if (currentCategoryId.HasValue &&
                    currentParentId.Value == currentCategoryId.Value)
                {
                    throw new InvalidOperationException(
                        "Circular category relationship is not allowed");
                }

                if (!visited.Add(currentParentId.Value))
                {
                    throw new InvalidOperationException(
                        "Existing category hierarchy contains a circular relationship");
                }

                if (!categoryLookup.TryGetValue(
                    currentParentId.Value,
                    out var currentParent))
                {
                    throw new InvalidOperationException(
                        "Invalid category hierarchy");
                }

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
                await _unitOfWork.Category.HasActiveSubCategoriesAsync(
                    categoryId);

            if (hasActiveSubCategories)
            {
                throw new InvalidOperationException(
                    "Cannot deactivate a category that has active subcategories");
            }

            var hasPublishedProducts =
                await _unitOfWork.Category.HasPublishedProductsAsync(
                    categoryId);

            if (hasPublishedProducts)
            {
                throw new InvalidOperationException(
                    "Cannot deactivate a category that contains published products");
            }
        }

        public async Task<PagedResultDto> GetPagedCategoriesAsync(string? search, string? status, int page, int pageSize)
        {
            // Allow only supported page sizes.
            int[] allowedPageSizes = [10, 20, 50];

            if (!allowedPageSizes.Contains(pageSize))
            {
                pageSize = 10;
            }

            page = Math.Max(1, page);

            return await _unitOfWork.Category.GetPagedCategoriesAsync(search,
                status, page, pageSize);
        }

        public async Task<CategoryStatisticsDTo> GetStatisticsAsync()
        {
            return await _unitOfWork.Category.GetStatisticsAsync();
        }
    }
}