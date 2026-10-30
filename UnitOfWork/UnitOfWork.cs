using Veltrix.Data;
using Veltrix.Models;
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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        public IProductRepository Product { get; }

        public ICategoryRepository Category { get; }

        public IInventoryRepository Inventory { get; }

        public IInventoryTransactionRepository InventoryTransactions { get; }

        public IProductImageRepository ProductImages { get; }

        public IProductReviewRepository ProductReviews { get; }

        public ICartRepository Carts { get; }

        public ICartItemRepository CartItems { get; }

        public ICouponRepository Coupons { get; }

        public ICouponUsageRepository CouponUsages { get; }

        public IOrderRepository Orders { get; }

        public IOrderItemRepository OrderItems { get; }

        public IOrderStatusHistoryRepository OrdersStatusHistory { get; }

        public INotificationRepository Notifications { get; }

        public IUserAddressRepository UserAddresses { get; }

        public IApplicationUserRepository Users { get; }

        public IShippingSettingsRepository ShippingSettings { get; }

        public IStoreSettingsRepository StoreSettings { get; }

        public UnitOfWork(
            AppDbContext db,
             IProductRepository products,
            ICategoryRepository categories,
            IInventoryRepository inventory,
            IInventoryTransactionRepository inventoryTransactions,
            IProductImageRepository productImages,
            IProductReviewRepository productReviews,
            ICartRepository carts,
            ICartItemRepository cartItems,
            ICouponRepository coupons,
            ICouponUsageRepository couponUsages,
            IOrderRepository orders,
            IOrderItemRepository orderItems,
            IOrderStatusHistoryRepository ordersStatusHistory,
            INotificationRepository notifications,
            IUserAddressRepository userAddresses,
            IApplicationUserRepository users,
            IShippingSettingsRepository shippingSettings,
            IStoreSettingsRepository storeSettings)
        {
            _db = db;
            Product = products;
            Category = categories;
            Inventory = inventory;
            InventoryTransactions = inventoryTransactions;
            ProductImages = productImages;
            ProductReviews = productReviews;

            Carts = carts;
            CartItems = cartItems;

            Coupons = coupons;
            CouponUsages = couponUsages;

            Orders = orders;
            OrderItems = orderItems;
            OrdersStatusHistory = ordersStatusHistory;

            Notifications = notifications;
            UserAddresses = userAddresses;
            Users = users;

            ShippingSettings = shippingSettings;
            StoreSettings = storeSettings;
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
