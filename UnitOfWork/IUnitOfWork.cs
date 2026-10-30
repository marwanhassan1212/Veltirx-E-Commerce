using Veltrix.Repositories.Cart;
using Veltrix.Repositories.CartItem;
using Veltrix.Repositories.Category;
using Veltrix.Repositories.Coupon;
using Veltrix.Repositories.CouponUsage;
using Veltrix.Repositories.Inventory;
using Veltrix.Repositories.InventoryTransaction;
using Veltrix.Repositories.Notification;
using Veltrix.Repositories.Order;
using Veltrix.Repositories.OrderItem;
using Veltrix.Repositories.OrderStatusHistory;
using Veltrix.Repositories.Product;
using Veltrix.Repositories.ProductImage;
using Veltrix.Repositories.ProductReview;
using Veltrix.Repositories.ShippingSettings;
using Veltrix.Repositories.StoreSettings;
using Veltrix.Repositories.User;
using Veltrix.Repositories.UserAddress;

namespace Veltrix.UnitOfWork
{
    public interface IUnitOfWork
    {
        IProductRepository Product { get; }
        ICategoryRepository Category { get; }
        IInventoryRepository Inventory { get; }
        IInventoryTransactionRepository InventoryTransactions { get; }
        IProductImageRepository ProductImages { get; }
        IProductReviewRepository ProductReviews { get; }
        ICartRepository Carts { get; }
        ICartItemRepository CartItems { get; }
        ICouponRepository Coupons { get; }
        ICouponUsageRepository CouponUsages { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        IOrderStatusHistoryRepository OrdersStatusHistory { get; }
        INotificationRepository Notifications { get; }
        IUserAddressRepository UserAddresses { get; }
        IApplicationUserRepository Users { get; }
        IShippingSettingsRepository ShippingSettings { get; }
        IStoreSettingsRepository StoreSettings { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
