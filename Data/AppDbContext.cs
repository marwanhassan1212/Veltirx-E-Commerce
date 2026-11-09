using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Veltrix.Models;

namespace Veltrix.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Coupon> Coupons { get; set; } = null!;
        public DbSet<CouponUsage> CouponUsages { get; set; } = null!;
        public DbSet<Inventory> Inventories { get; set; } = null!;
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductImage> ProductImages { get; set; } = null!;
        public DbSet<ProductReview> ProductReviews { get; set; } = null!;
        public DbSet<ShippingSettings> ShippingSettings { get; set; } = null!;
        public DbSet<StoreSettings> StoreSettings { get; set; } = null!;
        public DbSet<UserAddress> UserAddresses { get; set; } = null!;
        public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
        public DbSet<VariantOptions> VariantOptions { get; set; } = null!;
        public DbSet<VariantOptionValue> VariantOptionValues { get; set; } = null!;
        public DbSet<ProductVariantOption> ProductVariantOptions { get; set; } = null!;
        public DbSet<ProductVariantImage> ProductVariantImages { get; set; } = null!;
        public DbSet<OrderFulfillment> OrderFulfillments { get; set; }
        public DbSet<FulfillmentItem> FulfillmentItems { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentTrackingEvent> ShipmentTrackingEvents { get; set; }
        public DbSet<OrderReturn> OrderReturns { get; set; }
        public DbSet<OrderReturnItem> OrderReturnItems { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<CodReconciliation> CodReconciliations { get; set; }
        public DbSet<CodReconciliationItem> CodReconciliationItems { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
