using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            // Table and Constraints
            builder.ToTable("OrderItems", table =>
            {
                table.HasCheckConstraint(
                    "CK_OrderItems_Quantity",
                    "[Quantity] > 0");

                table.HasCheckConstraint(
                    "CK_OrderItems_UnitPrice",
                    "[UnitPrice] >= 0");

                table.HasCheckConstraint(
                    "CK_OrderItems_LineTotal",
                    "[LineTotal] >= 0");
            });

            // Keys
            builder.HasKey(x => x.Id);

            //Properties
            builder.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.SKU).IsRequired().HasMaxLength(50);
            builder.Property(x => x.UnitPrice).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.LineTotal).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();

            //Indexes
            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.ProductId);

            //RelationShips
            builder.HasOne(x => x.Order).WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.OrderId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product).WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.ProductId).IsRequired().OnDelete(DeleteBehavior.Restrict);
        }
    }
}
