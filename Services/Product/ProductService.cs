using Veltrix.DTOs.Common;
using Veltrix.DTOs.ProductDTO;
using Veltrix.DTOs.Storefront;
using Veltrix.Repositories.Product;
using Veltrix.UnitOfWork;

namespace Veltrix.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // Mapping
        private static ProductDto MapToDto(Models.Product product)
        {
            var defaultVariant = product.Variants
                .FirstOrDefault(x => x.IsDefault && x.IsActive);

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,

                SKU = defaultVariant?.SKU ?? string.Empty,
                Price = defaultVariant?.Price ?? 0,

                ShortDescription = product.ShortDescription,
                Description = product.Description,
                IsPublished = product.IsPublished,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,

                ImageUrls = product.ProductImages
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => x.ImageUrl)
                    .ToList(),

                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
        {
            // Check Slug
            var slug = await unitOfWork.Product.GetBySlugAsync(createProductDto.Slug);
            if (slug != null)
                throw new InvalidOperationException("Slug already exists");

            // Check Category
            var category = await unitOfWork.Category.GetByIdAsync(createProductDto.CategoryId);
            if (category == null)
                throw new InvalidOperationException("Category does not exist");

            var product = new Models.Product
            {
                Name = createProductDto.Name,
                Slug = createProductDto.Slug,
                ShortDescription = createProductDto.ShortDescription,
                Description = createProductDto.Description,
                CategoryId = createProductDto.CategoryId,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow,
                Variants = new List<Models.ProductVariant>()
            };


            await unitOfWork.Product.AddAsync(product);
            await unitOfWork.SaveChangesAsync();

            product.Category = category;
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                SKU = string.Empty,
                Price = 0,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                IsPublished = product.IsPublished,
                CategoryId = product.CategoryId,
                CategoryName = category.Name,
                ImageUrls = new List<string>(),
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

        }

        public async Task<IReadOnlyList<ProductDto>> GetAllAsync()
        {
            var products = await unitOfWork.Product.GetAllAsync();
            return products.Select(MapToDto).ToList();

        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await unitOfWork.Product.GetByIdAsync(id);
            if (product == null)
                return null;
            return MapToDto(product);
                
        }

        public async Task<ProductDto?> GetBySlugAsync(string slug)
        {
            var product = await unitOfWork.Product.GetBySlugAsync(slug);
            if (product == null)
                return null;
            return MapToDto(product);
        }

        public async Task<PagedResultDto<ProductDto>> GetPagedAsync(int pageNumber, int pageSize, string? search, string? status)
        {
            // Validate Page Number
            pageNumber = Math.Max(1, pageNumber);

            // Validate Page Size
            var allowedPageSizes = new[] { 10, 20, 50 };

            if (!allowedPageSizes.Contains(pageSize))
                pageSize = 10;

            // Get Paged Products
            var result = await unitOfWork.Product.GetPagedAsync(pageNumber, pageSize, search,
                status);

            // Map Products to DTOs
            return new PagedResultDto<ProductDto>
            {
                Items = result.Items
                    .Select(MapToDto)
                    .ToList(),

                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<IReadOnlyList<ProductDto>> GetPublishedProductsAsync()
        {
            var products = await unitOfWork.Product.GetPublishedProductsAsync();
            return products.Select(MapToDto).ToList();


        }

        public async Task<StorefrontProductDto?> GetStorefrontProductAsync(string slug)
        {
            var product = await unitOfWork.Product
                .GetPublishedForStorefrontAsync(slug);

            if (product == null)
                return null;

            var defaultVariant = product.Variants
                .FirstOrDefault(x => x.IsDefault)
                ?? product.Variants.FirstOrDefault();

            var dto = new StorefrontProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                CategoryName = product.Category.Name,

                Price = defaultVariant?.Price ?? 0,
                SKU = defaultVariant?.SKU ?? string.Empty,

                Images = product.ProductImages
                    .OrderByDescending(x => x.IsPrimary)
                    .ThenBy(x => x.DisplayOrder)
                    .Select(x => new StorefrontProductImageDto
                    {
                        Id = x.Id,
                        ImageUrl = x.ImageUrl,
                        AltText = x.AltText,
                        IsPrimary = x.IsPrimary,
                        DisplayOrder = x.DisplayOrder
                    })
                    .ToList(),

                Variants = product.Variants
                    .Select(v => new StorefrontVariantDto
                    {
                        Id = v.Id,
                        SKU = v.SKU,
                        Price = v.Price,
                        IsDefault = v.IsDefault,

                        OptionValueIds = v.VariantOptions
                            .Select(vo => vo.VariantOptionValueId)
                            .ToList(),

                        Images = v.Images
                            .OrderByDescending(i => i.IsPrimary)
                            .ThenBy(i => i.DisplayOrder)
                            .Select(i => new StorefrontProductImageDto
                            {
                                Id = i.Id,
                                ImageUrl = i.ImageUrl,
                                AltText = i.AltText,
                                IsPrimary = i.IsPrimary,
                                DisplayOrder = i.DisplayOrder
                            })
                            .ToList()
                    })
                    .ToList(),

                Options = product.Variants
                    .SelectMany(v => v.VariantOptions)
                    .GroupBy(vo => new
                    {
                        vo.VariantOptionValue.VariantOption.Id,
                        vo.VariantOptionValue.VariantOption.Name,
                        vo.VariantOptionValue.VariantOption.DisplayOrder
                    })
                    .Select(g => new StorefrontOptionDto
                    {
                        Id = g.Key.Id,
                        Name = g.Key.Name,
                        DisplayOrder = g.Key.DisplayOrder,

                        Values = g
                            .Select(vo => vo.VariantOptionValue)
                            .DistinctBy(v => v.Id)
                            .OrderBy(v => v.DisplayOrder)
                            .Select(v => new StorefrontOptionValueDto
                            {
                                Id = v.Id,
                                Value = v.Value,
                                DisplayOrder = v.DisplayOrder
                            })
                            .ToList()
                    })
                    .OrderBy(x => x.DisplayOrder)
                    .ToList()
            };

            return dto;
        }

        public async Task<bool> PublishAsync(int id)
        {
            var product = await unitOfWork.Product.GetByIdAsync(id);
            if (product == null)
                return false;

            if (product.IsPublished)
                return true;

            // Check Default Variant
            var defaultVariant = product.Variants
                .FirstOrDefault(x => x.IsDefault);

            if (defaultVariant == null || !defaultVariant.IsActive)
                throw new InvalidOperationException(
                    "Product must have an active default variant before publishing.");


            product.IsPublished = true;
            product.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnpublishAsync(int id)
        {
            var product = await unitOfWork.Product.GetByIdAsync(id);
            if (product == null)
                return false;

            if (!product.IsPublished)
                return true;

            product.IsPublished = false;
            product.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Product.Update(product);
            await unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<ProductDto?> UpdateAsync(int id , UpdateProductDto updateProductDto)
        {
            // Check Product
            var product = await unitOfWork.Product.GetByIdAsync(id);
            if (product == null)
                return null;

            // Check Slug
            var existingSlug = await unitOfWork.Product.GetBySlugAsync(updateProductDto.Slug);
            if (existingSlug != null && existingSlug.Id != id)
                throw new InvalidOperationException("Slug already exists");

            // Check Category
            var category = await unitOfWork.Category.GetByIdAsync(updateProductDto.CategoryId);
            if (category == null)
                throw new InvalidOperationException("Category does not exist");

            product.Name = updateProductDto.Name;
            product.Slug = updateProductDto.Slug;
            product.ShortDescription = updateProductDto.ShortDescription;
            product.Description = updateProductDto.Description;
            product.CategoryId = updateProductDto.CategoryId;
            product.Category = category;
            product.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync();
            return MapToDto(product);
        }
    }
}
