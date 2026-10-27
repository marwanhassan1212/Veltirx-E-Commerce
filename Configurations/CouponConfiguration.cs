using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            // Table and Constraints
            builder.ToTable("Coupons", table =>
            {
                table.HasCheckConstraint("CK_Coupons_DiscountValue", "[DiscountValue] > 0");
                table.HasCheckConstraint("CK_Coupons_MinimumOrderAmount"
                    , "[MinimumOrderAmount] IS NULL OR [MinimumOrderAmount] >= 0");
                table.HasCheckConstraint("CK_Coupons_MaximumDiscountAmount"
                    , "[MaximumDiscountAmount] IS NULL OR [MaximumDiscountAmount] > 0");
                table.HasCheckConstraint("CK_Coupons_UsageLimit"
                    , "[UsageLimit] IS NULL OR [UsageLimit] > 0");
                table.HasCheckConstraint("CK_Coupons_UsageCount"
                    , "[UsageCount] >= 0");
                table.HasCheckConstraint("CK_Coupons_DiscountType" 
                    , "([DiscountType] = 'Percentage' AND [DiscountValue] <= 100) OR " +
                      "([DiscountType] = 'FixedAmount')");
                table.HasCheckConstraint("CK_Coupons_ValidDates", "[ExpiresAt] > [StartsAt]");
            });

            // Keys
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.DiscountType).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(x => x.DiscountValue).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.MinimumOrderAmount).HasPrecision(18, 2);
            builder.Property(x => x.MaximumDiscountAmount).HasPrecision(18, 2);
            builder.Property(x => x.UsageLimit);
            builder.Property(x => x.UsageCount).HasDefaultValue(0).IsRequired();
            builder.Property(x => x.StartsAt).IsRequired();
            builder.Property(x => x.ExpiresAt).IsRequired();
            builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            //Indexes
            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasIndex(x => new { x.IsActive, x.StartsAt, x.ExpiresAt });

            //RelationShips
            builder.HasMany(x => x.Orders).WithOne(x => x.Coupon)
                .HasForeignKey(x => x.CouponId).IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.CouponUsages).WithOne(x => x.Coupon)
                 .HasForeignKey(x => x.CouponId).IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
