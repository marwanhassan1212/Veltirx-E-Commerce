using Microsoft.EntityFrameworkCore.Storage;
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
        IOrderFulfillmentRepository OrderFulfillments { get; }
        IFulfillmentItemRepository FulfillmentItems { get; }
        IShipmentRepository Shipments { get; }
        IShipmentTrackingEventRepository ShipmentTrackingEvents { get; }
        IOrderReturnRepository OrderReturns { get; }
        IOrderReturnItemRepository OrderReturnItems { get; }
        IRefundRepository Refunds { get; }
        ICodReconciliationRepository CodReconciliations { get; }
        ICodReconciliationItemRepository CodReconciliationItems { get; }
        INotificationRepository Notifications { get; }
        IUserAddressRepository UserAddresses { get; }
        IApplicationUserRepository Users { get; }
        IShippingSettingsRepository ShippingSettings { get; }
        IStoreSettingsRepository StoreSettings { get; }
        IProductVariantRepository productVariant { get; }
        IVariantOptionValueRepository VariantOptionValues { get; }
        IVariantOptionsRepository VariantOptions { get; }
        IProductVariantImageRepository ProductVariantImages { get; }

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);

        Task<IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default);
    }
}