
using Microsoft.EntityFrameworkCore;
using Veltrix.Data;

namespace Veltrix.Repositories.ProductReview
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly AppDbContext _db;
        public ProductReviewRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Models.ProductReview productReview)
        {
            await _db.ProductReviews.AddAsync(productReview);
        }

        public async Task<IReadOnlyList<Models.ProductReview>> GetApprovedByProductIdAsync(int productId)
        {
            return await _db.ProductReviews
                  .AsNoTracking()
                  .Where(x => x.ProductId == productId && x.IsApproved)
                  .OrderByDescending(x => x.CreatedAt)
                  .ThenByDescending(x => x.Id)
                  .ToListAsync();
        }

        public async Task<Models.ProductReview?> GetByIdAsync(long id)
        {
            return await _db.ProductReviews
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Models.ProductReview?> GetByOrderItemIdAsync(int orderItemId)
        {
            return await _db.ProductReviews.AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrderItemId == orderItemId);
        }

        public async Task<IReadOnlyList<Models.ProductReview>> GetByProductIdAsync(int productId)
        {
            return await _db.ProductReviews.Where(x => x.ProductId == productId)
                 .AsNoTracking().OrderByDescending(x => x.CreatedAt)
                 .ThenByDescending(x => x.Id).ToListAsync();
        }

        public async Task<IReadOnlyList<Models.ProductReview>> GetByUserIdAsync(string userId)
        {
            return await _db.ProductReviews.Where(x => x.UserId == userId)
                .AsNoTracking().OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                .ToListAsync();
        }

        public void Update(Models.ProductReview productReview)
        {
            _db.ProductReviews.Update(productReview);
        }
    }
}
