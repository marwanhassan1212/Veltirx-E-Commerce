using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
    {
        public void Configure(EntityTypeBuilder<CouponUsage> builder)
        {
            // Table and Constraints
            builder.ToTable("CouponUsages", tables =>
            tables.HasCheckConstraint("CK_DiscountAmount_CouponUsages", "[DiscountAmount] >= 0"));

            // Keys
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.DiscountAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.UsedAt).IsRequired();
            builder.Property(x => x.CouponId).IsRequired();
            builder.Property(x => x.UsedByUserId).IsRequired().HasMaxLength(128);
            builder.Property(x => x.OrderId);

            //Indexes
            builder.HasIndex(x => new { x.CouponId, x.UsedByUserId }).IsUnique();
            builder.HasIndex(x => x.UsedByUserId);

            // RelationShips
            builder.HasOne(x => x.Coupon).WithMany(x => x.CouponUsages)
                .HasForeignKey(x => x.CouponId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UsedBy).WithMany(x => x.CouponUsages)
                .HasForeignKey(x => x.UsedByUserId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Order).WithOne(x => x.CouponUsage).HasForeignKey<CouponUsage>(x => x.OrderId)
                .IsRequired().OnDelete(DeleteBehavior.Restrict);





        }
    }
}
