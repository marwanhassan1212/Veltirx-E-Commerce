using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class OrderFulfillmentConfiguration
        : IEntityTypeConfiguration<OrderFulfillment>
    {
        public void Configure(EntityTypeBuilder<OrderFulfillment> builder)
        {
            // Table
            builder.ToTable("OrderFulfillments", table =>
            {
                table.HasCheckConstraint(
                    "CK_OrderFulfillments_CompletedAt",
                    "[CompletedAt] IS NULL OR [CompletedAt] >= [CreatedAt]");
            });
            //Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.FulfillmentNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Status).IsRequired().HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.Property(x => x.CreatedAt).IsRequired();

            // Indexes
            builder.HasIndex(x => x.FulfillmentNumber).IsUnique();
            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.Status);

            // RelationShips
            builder.HasOne(x => x.Order).WithMany(x => x.Fulfillments)
                .HasForeignKey(x => x.OrderId).IsRequired().OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(x => x.Items).WithOne(x => x.Fulfillment)
                .HasForeignKey(x => x.FulfillmentId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}