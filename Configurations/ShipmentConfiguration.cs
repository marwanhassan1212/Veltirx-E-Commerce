using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.ToTable("Shipments", table =>
            {
                table.HasCheckConstraint(
                    "CK_Shipments_DeliveredAt",
                    "[DeliveredAt] IS NULL OR [ShippedAt] IS NOT NULL");

                table.HasCheckConstraint(
                    "CK_Shipments_ShippedAt",
                    "[ShippedAt] IS NULL OR [ShippedAt] >= [CreatedAt]");
            });

            // Key
            builder.HasKey(x => x.Id);
            
            // Properties
            builder.Property(x => x.TrackingNumber).IsRequired().HasMaxLength(100);
            builder.Property(x => x.CarrierName).HasMaxLength(150);
            builder.Property(x => x.Status).IsRequired().HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.HasIndex(x => x.TrackingNumber).IsUnique();


            // Indexes
            builder.HasIndex(x => x.OrderId).IsUnique();
            builder.HasIndex(x => x.Status);

            // Relationsships
            builder.HasOne(x => x.Order).WithOne(x => x.Shipment)
                .HasForeignKey<Shipment>(x => x.OrderId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.TrackingEvents).WithOne(x => x.Shipment)
                .HasForeignKey(x => x.ShipmentId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}