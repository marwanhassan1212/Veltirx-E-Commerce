using Microsoft.EntityFrameworkCore.Storage;
using Veltrix.Data;

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
using Veltrix.Repositories.OrderFulfillment;
using Veltrix.Repositories.FulfillmentItem;
using Veltrix.Repositories.Shipment;
using Veltrix.Repositories.ShipmentTrackingEvent;
using Veltrix.Repositories.OrderReturn;
using Veltrix.Repositories.OrderReturnItem;
using Veltrix.Repositories.Refund;
using Veltrix.Repositories.CodReconciliation;
using Veltrix.Repositories.CodReconciliationItem;

using Veltrix.Repositories.Product;
using Veltrix.Repositories.ProductImage;
using Veltrix.Repositories.ProductReview;
using Veltrix.Repositories.ProductVariant;
using Veltrix.Repositories.ProductVariantImage;

using Veltrix.Repositories.ShippingSettings;
using Veltrix.Repositories.StoreSettings;
using Veltrix.Repositories.User;
using Veltrix.Repositories.UserAddress;
using Veltrix.Repositories.VariantOptions;
using Veltrix.Repositories.VariantOptionValue;

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
        public IOrderFulfillmentRepository OrderFulfillments { get; }
        public IFulfillmentItemRepository FulfillmentItems { get; }
        public IShipmentRepository Shipments { get; }
        public IShipmentTrackingEventRepository ShipmentTrackingEvents { get; }
        public IOrderReturnRepository OrderReturns { get; }
        public IOrderReturnItemRepository OrderReturnItems { get; }
        public IRefundRepository Refunds { get; }
        public ICodReconciliationRepository CodReconciliations { get; }
        public ICodReconciliationItemRepository CodReconciliationItems { get; }
        public INotificationRepository Notifications { get; }
        public IUserAddressRepository UserAddresses { get; }
        public IApplicationUserRepository Users { get; }
        public IShippingSettingsRepository ShippingSettings { get; }
        public IStoreSettingsRepository StoreSettings { get; }
        public IProductVariantRepository productVariant { get; }
        public IProductVariantImageRepository ProductVariantImages { get; }
        public IVariantOptionValueRepository VariantOptionValues { get; }
        public IVariantOptionsRepository VariantOptions { get; }

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
            IOrderFulfillmentRepository orderFulfillments,
            IFulfillmentItemRepository fulfillmentItems,
            IShipmentRepository shipments,
            IShipmentTrackingEventRepository shipmentTrackingEvents,
            IOrderReturnRepository orderReturns,
            IOrderReturnItemRepository orderReturnItems,
            IRefundRepository refunds,
            ICodReconciliationRepository codReconciliations,
            ICodReconciliationItemRepository codReconciliationItems,
            INotificationRepository notifications,
            IUserAddressRepository userAddresses,
            IApplicationUserRepository users,
            IShippingSettingsRepository shippingSettings,
            IStoreSettingsRepository storeSettings,
            IProductVariantRepository productVariant,
            IVariantOptionValueRepository variantOptionValues,
            IVariantOptionsRepository variantOptions,
            IProductVariantImageRepository productVariantImages)
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

            OrderFulfillments = orderFulfillments;
            FulfillmentItems = fulfillmentItems;

            Shipments = shipments;
            ShipmentTrackingEvents = shipmentTrackingEvents;

            OrderReturns = orderReturns;
            OrderReturnItems = orderReturnItems;

            Refunds = refunds;

            CodReconciliations = codReconciliations;
            CodReconciliationItems = codReconciliationItems;

            Notifications = notifications;

            UserAddresses = userAddresses;
            Users = users;

            ShippingSettings = shippingSettings;
            StoreSettings = storeSettings;

            this.productVariant = productVariant;
            VariantOptionValues = variantOptionValues;
            VariantOptions = variantOptions;
            ProductVariantImages = productVariantImages;
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            return await _db.Database.BeginTransactionAsync(cancellationToken);
        }
    }
}