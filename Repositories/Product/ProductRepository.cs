using Microsoft.EntityFrameworkCore;
using Veltrix.Data;
using Veltrix.DTOs.Common;
using Veltrix.Models;

namespace Veltrix.Repositories.Product
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;
        public ProductRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Models.Product>> GetAllAsync()
        {
            return await _db.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.Variants)
                .AsNoTracking()
                .ToListAsync();
                    
        }

        public async Task<Models.Product?> GetByIdAsync(int Id)
        {
            return await _db.Products
              .Include(x => x.Category)
              .Include(x => x.ProductImages)
              .Include(x => x.Variants)
              .FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Models.Product?> GetBySlugAsync(string slug)
        {
            return await _db.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.Variants)
                .FirstOrDefaultAsync(x => x.Slug == slug);
        }

        public async Task<IReadOnlyList<Models.Product>> GetPublishedProductsAsync()
        {
            return await _db.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.Variants)
                .Where(x => x.IsPublished)
                .AsNoTracking()
                .ToListAsync();
        }

        public void Update(Models.Product product)
        {
            _db.Products.Update(product);
        }

        public async Task AddAsync(Models.Product product)
        {
            await _db.Products.AddAsync(product);
        }

        public async Task<PagedResultDto<Models.Product>> GetPagedAsync(int pageNumber, int pageSize, string? search, string? status)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _db.Products.AsQueryable();

            if(!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                x.Name.Contains(search) ||
                x.Slug.Contains(search) ||
                x.Category.Name.Contains(search) ||
                x.Variants.Any(v => v.SKU.Contains(search))
                );
            }

            if(!string.IsNullOrWhiteSpace(status))
            {
                switch(status.Trim().ToLower())
                {
                    case "published":
                        query = query.Where(x => x.IsPublished);
                        break;
                    case "unpublished":
                        query = query.Where(x => !x.IsPublished);
                        break;
                }
            }

            var TotalCount = await query.CountAsync();

            var products = await query.Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.Variants)
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<Models.Product>
            {
                Items = products,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = TotalCount
            };
        }

        public async Task<Models.Product?> GetPublishedForStorefrontAsync(string slug)
        {
            return await _db.Products
                .AsNoTracking()
                .Where(x => x.Slug == slug && x.IsPublished)
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.Variants.Where(v => v.IsActive))
                    .ThenInclude(v => v.VariantOptions)
                        .ThenInclude(vo => vo.VariantOptionValue)
                            .ThenInclude(vov => vov.VariantOption)
                .Include(x => x.Variants.Where(v => v.IsActive))
                    .ThenInclude(v => v.Images)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }
    }
}
