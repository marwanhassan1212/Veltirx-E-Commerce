using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class FulfillmentItemConfiguration
        : IEntityTypeConfiguration<FulfillmentItem>
    {
        public void Configure(EntityTypeBuilder<FulfillmentItem> builder)
        {
            // Table
            builder.ToTable("FulfillmentItems", table =>
            {
                table.HasCheckConstraint(
                    "CK_FulfillmentItems_Quantity",
                    "[Quantity] > 0");
            });

            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Quantity).IsRequired();


            // Indexes
            builder.HasIndex(x => new
            {
                x.FulfillmentId,
                x.OrderItemId
            }).IsUnique();

            builder.HasIndex(x => x.OrderItemId);

            // Relationships
            builder.HasOne(x => x.Fulfillment).WithMany(x => x.Items)
                .HasForeignKey(x => x.FulfillmentId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OrderItem).WithMany().HasForeignKey(x => x.OrderItemId)
                .IsRequired().OnDelete(DeleteBehavior.Restrict);



        }
    }
}