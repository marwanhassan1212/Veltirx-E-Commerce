using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            // Keys
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.SKU).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Price).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.IsDefault).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);

            // Constraints
            builder.ToTable("ProductVariants", table =>
            {
                table.HasCheckConstraint(
                    "CK_ProductVariants_Price",
                    "[Price] >= 0");
            });

            // Indexes
            builder.HasIndex(x => x.SKU)
                .IsUnique();

            builder.HasIndex(x => x.ProductId)
                .IsUnique()
                .HasFilter("[IsDefault] = 1")
                .HasDatabaseName("IX_ProductVariants_ProductId_Default");

            // Relationships
            builder.HasOne(x => x.Product).WithMany(x => x.Variants)
                .HasForeignKey(x => x.ProductId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
