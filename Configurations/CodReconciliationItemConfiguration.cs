using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class CodReconciliationItemConfiguration
        : IEntityTypeConfiguration<CodReconciliationItem>
    {
        public void Configure(EntityTypeBuilder<CodReconciliationItem> builder)
        {

            // Table
            builder.ToTable("CodReconciliationItems", table =>
            {
                table.HasCheckConstraint(
                    "CK_CodReconciliationItems_Amounts",
                    "[ExpectedAmount] >= 0 AND [CollectedAmount] >= 0");

                table.HasCheckConstraint(
                    "CK_CodReconciliationItems_Difference",
                    "[DifferenceAmount] = [CollectedAmount] - [ExpectedAmount]");
            });

            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.ExpectedAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.CollectedAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.DifferenceAmount).HasPrecision(18, 2).IsRequired();

            // Indexes
            builder.HasIndex(x => x.ReconciliationId);           
            builder.HasIndex(x => x.OrderId).IsUnique();

            // Reltaionships
            builder.HasOne(x => x.Reconciliation)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.ReconciliationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}