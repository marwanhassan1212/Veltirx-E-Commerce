using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class OrderReturnItemConfiguration
        : IEntityTypeConfiguration<OrderReturnItem>
    {
        public void Configure(EntityTypeBuilder<OrderReturnItem> builder)
        {

            // Table
            builder.ToTable("OrderReturnItems", table =>
            {
                table.HasCheckConstraint(
                    "CK_OrderReturnItems_Quantity",
                    "[Quantity] > 0");
            });

            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Quantity).IsRequired();
            builder.Property(x => x.ConditionNotes).HasMaxLength(1000);

            // Indexes
            builder.HasIndex(x => new
            {
                x.ReturnId,
                x.OrderItemId
            }).IsUnique();
            builder.HasIndex(x => x.OrderItemId);


            // Relationships
            builder.HasOne(x => x.Return)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.ReturnId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OrderItem)
                .WithMany()
                .HasForeignKey(x => x.OrderItemId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}