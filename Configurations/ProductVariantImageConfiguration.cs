using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class ProductVariantImageConfiguration
        : IEntityTypeConfiguration<ProductVariantImage>
    {
        public void Configure(EntityTypeBuilder<ProductVariantImage> builder)
        {
            // Keys
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(x => x.AltText)
                .HasMaxLength(300);

            builder.Property(x => x.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(x => x.ProductVariantId)
                .IsUnique()
                .HasFilter("[IsPrimary] = 1")
                .HasDatabaseName("IX_ProductVariantImages_VariantId_Primary");

            // Relationships
            builder.HasOne(x => x.ProductVariant)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.ProductVariantId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Table
            builder.ToTable("ProductVariantImages");
        }
    }
}