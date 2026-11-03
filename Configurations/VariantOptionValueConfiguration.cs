using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class VariantOptionValueConfiguration
        : IEntityTypeConfiguration<VariantOptionValue>
    {
        public void Configure(EntityTypeBuilder<VariantOptionValue> builder)
        {
            // Keys
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Value).IsRequired().HasMaxLength(100);
            builder.Property(x => x.DisplayOrder).IsRequired().HasDefaultValue(0);

            // Constraints
            builder.ToTable("VariantOptionValues", table =>
            {
                table.HasCheckConstraint(
                    "CK_VariantOptionValues_DisplayOrder",
                    "[DisplayOrder] >= 0");
            });

            // Indexes
            builder.HasIndex(x => new { x.VariantOptionId, x.Value }).IsUnique();
            builder.HasIndex(x => x.VariantOptionId);

            // Relationships
            builder.HasOne(x => x.VariantOption)
                .WithMany(x => x.Values)
                .HasForeignKey(x => x.VariantOptionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}