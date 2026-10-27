using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
    {
        public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
        {
            // Table and Constraints
            builder.ToTable("InventoryTransactions", table =>
            {
                table.HasCheckConstraint(
                    "CK_InventoryTransactions_QuantityChange",
                    "[QuantityChange] <> 0");

                table.HasCheckConstraint(
                    "CK_InventoryTransactions_QuantityBefore",
                    "[QuantityBefore] >= 0");

                table.HasCheckConstraint(
                    "CK_InventoryTransactions_QuantityAfter",
                    "[QuantityAfter] >= 0");

                table.HasCheckConstraint(
                    "CK_InventoryTransactions_QuantityConsistency",
                    "[QuantityAfter] = [QuantityBefore] + [QuantityChange]");
            });

            // Keys
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.TransactionType).IsRequired().HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.QuantityChange).IsRequired();
            builder.Property(x => x.QuantityBefore).IsRequired();
            builder.Property(x => x.QuantityAfter).IsRequired();
            builder.Property(x => x.Reason).HasMaxLength(500);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.InventoryId).IsRequired();


            //Indexes
            builder.HasIndex(x => new { x.InventoryId, x.CreatedAt });
            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.PerformedByUserId);


            // RelationShips
            builder.HasOne(x => x.Inventory).WithMany(x => x.InventoryTransactions)
                .HasForeignKey(x => x.InventoryId).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PerformedByUser).WithMany(x => x.InventoryTransactions)
                .HasForeignKey(x => x.PerformedByUserId).IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Order).WithMany(x => x.InventoryTransactions).HasForeignKey(x => x.OrderId)
                .IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
