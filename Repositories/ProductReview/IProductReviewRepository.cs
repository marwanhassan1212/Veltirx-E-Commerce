namespace Veltrix.Repositories.ProductReview
{
    public interface IProductReviewRepository
    {
        Task<Models.ProductReview?> GetByIdAsync(long id);
        Task<IReadOnlyList<Models.ProductReview>> GetByProductIdAsync(int productId);
        Task<IReadOnlyList<Models.ProductReview>> GetApprovedByProductIdAsync(int productId);
        Task<IReadOnlyList<Models.ProductReview>> GetByUserIdAsync(string userId);
        Task<Models.ProductReview?> GetByOrderItemIdAsync(int orderItemId);
        Task AddAsync(Models.ProductReview productReview);
        void Update(Models.ProductReview productReview);
    }
}
