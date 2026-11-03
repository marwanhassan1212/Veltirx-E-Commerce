using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class ProductVariantOptionConfiguration
        : IEntityTypeConfiguration<ProductVariantOption>
    {
        public void Configure(EntityTypeBuilder<ProductVariantOption> builder)
        {
            // Keys
            builder.HasKey(x => x.Id);

            // Indexes
            builder.HasIndex(x => new
            {
                x.ProductVariantId,
                x.VariantOptionValueId
            }).IsUnique();
            builder.HasIndex(x => x.VariantOptionValueId);

            // Relationships
            builder.HasOne(x => x.ProductVariant)
                .WithMany(x => x.VariantOptions)
                .HasForeignKey(x => x.ProductVariantId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.VariantOptionValue)
                .WithMany(x => x.ProductVariantOptions)
                .HasForeignKey(x => x.VariantOptionValueId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}