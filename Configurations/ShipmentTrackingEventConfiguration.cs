using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class ShipmentTrackingEventConfiguration
        : IEntityTypeConfiguration<ShipmentTrackingEvent>
    {
        public void Configure(EntityTypeBuilder<ShipmentTrackingEvent> builder)
        {

            // Table
            builder.ToTable("ShipmentTrackingEvents");

            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.Property(x => x.Location).HasMaxLength(250);
            builder.Property(x => x.OccurredAt).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            // Indexes
            builder.HasIndex(x => new
            {
                x.ShipmentId,
                x.OccurredAt
            });

            // Relationships
            builder.HasOne(x => x.Shipment)
                .WithMany(x => x.TrackingEvents)
                .HasForeignKey(x => x.ShipmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}