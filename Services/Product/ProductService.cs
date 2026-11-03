using Veltrix.DTOs.ProductDTO;
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
                .FirstOrDefault(x => x.IsDefault);

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

            // Check SKU
            var sku = await unitOfWork.Product.GetBySkuAsync(createProductDto.SKU);
            if (sku != null)
                throw new InvalidOperationException("SKU already exists");

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
                Category = category,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow,
                Variants = new List<Models.ProductVariant>
                {
                    new Models.ProductVariant
                    {
                    SKU = createProductDto.SKU,
                    Price = createProductDto.Price,
                    IsDefault = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                    }
                }
            };


            await unitOfWork.Product.AddAsync(product);
            await unitOfWork.SaveChangesAsync();

            product.Category = category;
            return MapToDto(product);
            
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

        public async Task<IReadOnlyList<ProductDto>> GetPublishedProductsAsync()
        {
            var products = await unitOfWork.Product.GetPublishedProductsAsync();
            return products.Select(MapToDto).ToList();


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

            var defaultVariant = product.Variants.FirstOrDefault(x => x.IsDefault);
            if (defaultVariant == null)
                throw new InvalidOperationException("Default variant does not exist.");

            // Check Slug
            var existingSlug = await unitOfWork.Product.GetBySlugAsync(updateProductDto.Slug);
            if (existingSlug != null && existingSlug.Id != id)
                throw new InvalidOperationException("Slug already exists");

            // Check SKU
            var existingSKU = await unitOfWork.Product.GetBySkuAsync(updateProductDto.SKU);
            if (existingSKU != null && existingSKU.Id != defaultVariant.Id)
                throw new InvalidOperationException("SKU already exists");

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

            // Update Default Variant
            defaultVariant.SKU = updateProductDto.SKU;
            defaultVariant.Price = updateProductDto.Price;
            defaultVariant.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Product.Update(product);
            await unitOfWork.SaveChangesAsync();
            return MapToDto(product);
        }
    }
}
