using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class VariantOptionConfiguration
        : IEntityTypeConfiguration<VariantOptions>
    {
        public void Configure(EntityTypeBuilder<VariantOptions> builder)
        {
            // Keys
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.DisplayOrder).IsRequired().HasDefaultValue(0);

            // Constraints
            builder.ToTable("VariantOptions", table =>
            {
                table.HasCheckConstraint(
                    "CK_VariantOptions_DisplayOrder",
                    "[DisplayOrder] >= 0");
            });

            // Indexes
            builder.HasIndex(x => new { x.ProductId, x.Name }).IsUnique();
            builder.HasIndex(x => x.ProductId);

            // Relationships
            builder.HasOne(x => x.Product).WithMany(x => x.VariantOptions)
                .HasForeignKey(x => x.ProductId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Values).WithOne(x => x.VariantOption)
                .HasForeignKey(x => x.VariantOptionId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}