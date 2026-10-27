using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class ShippingSettingsConfiguration
        : IEntityTypeConfiguration<ShippingSettings>
    {
        public void Configure(EntityTypeBuilder<ShippingSettings> builder)
        {
            // Table and Constraints
            builder.ToTable("ShippingSettings", table =>
            {
                table.HasCheckConstraint(
                    "CK_ShippingSettings_ShippingFee",
                    "[ShippingFee] >= 0");
            });

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.ShippingFee).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            // Foreign Key
            builder.Property(x => x.UpdatedByUserId).HasMaxLength(128);

            // Indexes
            builder.HasIndex(x => x.IsActive);
            builder.HasIndex(x => x.UpdatedAt);
            builder.HasIndex(x => x.UpdatedByUserId);

            // Relationships
            builder.HasOne(x => x.UpdatedByUser)
                .WithMany(x => x.ShippingSettings)
                .HasForeignKey(x => x.UpdatedByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}