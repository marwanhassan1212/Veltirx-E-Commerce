using Microsoft.EntityFrameworkCore;
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
using Veltrix.Repositories.Product;
using Veltrix.Repositories.ProductImage;
using Veltrix.Repositories.ProductReview;
using Veltrix.Repositories.ShippingSettings;
using Veltrix.Repositories.StoreSettings;
using Veltrix.Repositories.User;
using Veltrix.Repositories.UserAddress;
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
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Constr")));

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


            // Unit of work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

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
