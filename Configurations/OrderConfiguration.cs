using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Table and Constraints
            builder.ToTable("Orders", table =>
            {
                table.HasCheckConstraint("CK_Orders_Amounts",
                "[Subtotal] >= 0 AND [DiscountAmount] >= 0 AND [ShippingFee] >= 0 AND [TotalAmount] >= 0");
                table.HasCheckConstraint("CK_Orders_Discount", "[DiscountAmount] <= [Subtotal]");
            });

            // Keys
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.OrderNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.OrderStatus).IsRequired().HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.PaymentMethod).IsRequired().HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.PaymentStatus).IsRequired().HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.Subtotal).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.DiscountAmount).HasPrecision(18, 2).HasDefaultValue(0).IsRequired();
            builder.Property(x => x.ShippingFee).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.TotalAmount).HasPrecision(18, 2).IsRequired();

            //Shipping Information
            builder.Property(x => x.ShippingRecipientName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.ShippingPhoneNumber).IsRequired().HasMaxLength(30);
            builder.Property(x => x.ShippingAddressLine1).IsRequired().HasMaxLength(250);
            builder.Property(x => x.ShippingAddressLine2).HasMaxLength(250);
            builder.Property(x => x.ShippingCity).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ShippingGovernorate).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ShippingPostalCode).HasMaxLength(20);
            builder.Property(x => x.ShippingInstructions).HasMaxLength(1000);
            builder.Property(x => x.CancelReason).HasMaxLength(1000);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);
            builder.Property(x => x.ConfirmedAt);
            builder.Property(x => x.ShippedAt);
            builder.Property(x => x.DeliveredAt);
            builder.Property(x => x.CancelledAt);



            //Indexes
            builder.HasIndex(x => x.OrderNumber).IsUnique();
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.CreatedAt);

            //Relations Ships
            builder.HasOne(x => x.User).WithMany(x => x.Orders).
                HasForeignKey(x => x.UserId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasOne(x => x.Coupon).WithMany(x => x.Orders).HasForeignKey(x => x.CouponId).IsRequired(false);

        }
    }
}
