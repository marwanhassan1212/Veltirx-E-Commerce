using Microsoft.EntityFrameworkCore;
using Veltrix.Data;
using Veltrix.Models;
using Veltrix.Repositories.Cart;
using Veltrix.Repositories.CartItem;
using Veltrix.Repositories.Category;
using Veltrix.Repositories.CodReconciliation;
using Veltrix.Repositories.CodReconciliationItem;
using Veltrix.Repositories.Coupon;
using Veltrix.Repositories.CouponUsage;
using Veltrix.Repositories.FulfillmentItem;
using Veltrix.Repositories.FullfillmentItem;
using Veltrix.Repositories.Inventory;
using Veltrix.Repositories.InventoryTransaction;
using Veltrix.Repositories.Notification;
using Veltrix.Repositories.Order;
using Veltrix.Repositories.OrderFulfillment;
using Veltrix.Repositories.OrderItem;
using Veltrix.Repositories.OrderReturn;
using Veltrix.Repositories.OrderReturnItem;
using Veltrix.Repositories.OrderStatusHistory;
using Veltrix.Repositories.Product;
using Veltrix.Repositories.ProductImage;
using Veltrix.Repositories.ProductReview;
using Veltrix.Repositories.ProductVariant;
using Veltrix.Repositories.ProductVariantImage;
using Veltrix.Repositories.Refund;
using Veltrix.Repositories.Shipment;
using Veltrix.Repositories.ShipmentTrackingEvent;
using Veltrix.Repositories.ShippingSettings;
using Veltrix.Repositories.StoreSettings;
using Veltrix.Repositories.User;
using Veltrix.Repositories.UserAddress;
using Veltrix.Repositories.VariantOptions;
using Veltrix.Repositories.VariantOptionValue;
using Veltrix.Services.Cart;
using Veltrix.Services.Category;
using Veltrix.Services.CodReconciliation;
using Veltrix.Services.CodReconciliationItem;
using Veltrix.Services.Coupon;
using Veltrix.Services.FileStorage;
using Veltrix.Services.FulfillmentItem;
using Veltrix.Services.Images;
using Veltrix.Services.Order;
using Veltrix.Services.OrderFulfillment;
using Veltrix.Services.OrderItem;
using Veltrix.Services.OrderReturn;
using Veltrix.Services.OrderReturnItem;
using Veltrix.Services.OrderStatusHistory;
using Veltrix.Services.Product;
using Veltrix.Services.ProductImage;
using Veltrix.Services.ProductVariant;
using Veltrix.Services.ProductVariantImage;
using Veltrix.Services.Refund;
using Veltrix.Services.Shipment;
using Veltrix.Services.ShipmentTrackingEvent;
using Veltrix.Services.ShippingSettings;
using Veltrix.Services.VariantOptions;
using Veltrix.Services.VariantOptionValues;
using Veltrix.UnitOfWork;

namespace Veltrix
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
            builder.Configuration.GetConnectionString("Constr"),
            sqlOptions => sqlOptions.UseCompatibilityLevel(110)));

            // Repositories
            builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICouponRepository, CouponRepository>();
            builder.Services.AddScoped<ICouponUsageRepository, CouponUsageRepository>();
            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
            builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            builder.Services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
            builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
            builder.Services.AddScoped<IShippingSettingsRepository, ShippingSettingsRepository>();
            builder.Services.AddScoped<IStoreSettingsRepository, StoreSettingsRepository>();
            builder.Services.AddScoped<IUserAddressRepository, UserAddressRepository>();
            builder.Services.AddScoped<IVariantOptionValueRepository, VariantOptionValueRepository>();
            builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            builder.Services.AddScoped<IProductVariantImageRepository, ProductVariantImageRepository>();
            builder.Services.AddScoped<IVariantOptionsRepository, VariantOptionsRepository>();
            builder.Services.AddScoped<IOrderFulfillmentRepository, OrderFulfillmentRepository>();
            builder.Services.AddScoped<IFulfillmentItemRepository, FullfillmentItemRepository>();
            builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
            builder.Services.AddScoped<IShipmentTrackingEventRepository, ShipmentTrackingEventRepository>();
            builder.Services.AddScoped<IOrderReturnRepository, OrderReturnRepository>();
            builder.Services.AddScoped<IOrderReturnItemRepository, OrderReturnItemRepository>();
            builder.Services.AddScoped<IRefundRepository, RefundRepository>();
            builder.Services.AddScoped<ICodReconciliationRepository, CodReconciliationRepository>();
            builder.Services.AddScoped<ICodReconciliationItemRepository, CodReconciliationItemRepository>();



            // Unit of work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            // Services
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
            builder.Services.AddScoped<IImageStorageService, LocalImageStorageService>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddScoped<IVariantOptionsService, VariantOptionsService>();
            builder.Services.AddScoped<IVariantOptionValueService, VariantOptionValueService>();
            builder.Services.AddScoped<IProductImageService, ProductImageService>();
            builder.Services.AddScoped<IProductVariantImageService, ProductVariantImageService>();        
            builder.Services.AddScoped<ICartService, CartService>();  
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderItemService, OrderItemService>();
            builder.Services.AddScoped<IOrderQueryService, OrderQueryService>();
            builder.Services.AddScoped<IOrderStatusHistoryService, OrderStatusHistoryService>();           
            builder.Services.AddScoped<IShipmentService, ShipmentService>();
            builder.Services.AddScoped<IShipmentTrackingEventService, ShipmentTrackingEventService>();          
            builder.Services.AddScoped<IOrderFulfillmentService, OrderFulfillmentService>();
            builder.Services.AddScoped<IFulfillmentItemService, FulfillmentItemService>();           
            builder.Services.AddScoped<IOrderReturnService, OrderReturnService>();
            builder.Services.AddScoped<IOrderReturnItemService, OrderReturnItemService>();          
            builder.Services.AddScoped<IRefundService, RefundService>();         
            builder.Services.AddScoped<IShippingSettingsService, ShippingSettingsService>();
            builder.Services.AddScoped<ICodReconciliationService, CodReconciliationService>();
            builder.Services.AddScoped<ICodReconciliationItemService, CodReconciliationItemService>();
            builder.Services.AddScoped<ICouponService, CouponService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
