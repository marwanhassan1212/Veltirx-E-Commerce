using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class StoreSettingsConfiguration : IEntityTypeConfiguration<StoreSettings>
    {
        public void Configure(EntityTypeBuilder<StoreSettings> builder)
        {
            // Keys
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.StoreName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.StoreDescription).HasMaxLength(500);
            builder.Property(x => x.SupportEmail).HasMaxLength(256);
            builder.Property(x => x.SupportPhone).HasMaxLength(30);
            builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(3);
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Property(x => x.UpdatedByUserId).HasMaxLength(128);
            builder.Property(x => x.LogoUrl).HasMaxLength(2048);
            builder.Property(x => x.IsStoreOpen).HasDefaultValue(true).IsRequired();
            builder.Property(x => x.MaintenanceMessage).HasMaxLength(1000);


            // Indexes
            builder.HasIndex(x => x.UpdatedAt);
            builder.HasIndex(x => x.UpdatedByUserId);

            // RelationsShips
            builder.HasOne(x => x.UpdatedByUser).WithMany(x => x.StoreSettings)
                .HasForeignKey(x => x.UpdatedByUserId).IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
