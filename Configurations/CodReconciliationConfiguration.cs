using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class CodReconciliationConfiguration
        : IEntityTypeConfiguration<CodReconciliation>
    {
        public void Configure(EntityTypeBuilder<CodReconciliation> builder)
        {

            // Table
            builder.ToTable("CodReconciliations", table =>
            {
                table.HasCheckConstraint(
                    "CK_CodReconciliations_Amounts",
                    "[ExpectedAmount] >= 0 AND [CollectedAmount] >= 0");

                table.HasCheckConstraint(
                    "CK_CodReconciliations_Difference",
                    "[DifferenceAmount] = [CollectedAmount] - [ExpectedAmount]");
            });

            // Key
            builder.HasKey(x => x.Id);


            // Properties
            builder.Property(x => x.ReconciliationNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ExpectedAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.CollectedAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.DifferenceAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.Status).IsRequired().HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.ReconciledByUserId).HasMaxLength(128);

            // Indexes
            builder.HasIndex(x => x.ReconciliationNumber).IsUnique();
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.CreatedAt);
            builder.HasIndex(x => x.ReconciledByUserId);
            
            // Relationships
            builder.HasOne(x => x.ReconciledByUser)
                .WithMany()
                .HasForeignKey(x => x.ReconciledByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Items)
                .WithOne(x => x.Reconciliation)
                .HasForeignKey(x => x.ReconciliationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}