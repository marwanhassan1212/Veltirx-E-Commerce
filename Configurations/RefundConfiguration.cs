using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class RefundConfiguration : IEntityTypeConfiguration<Refund>
    {
        public void Configure(EntityTypeBuilder<Refund> builder)
        {

            // Table
            builder.ToTable("Refunds", table =>
            {
                table.HasCheckConstraint(
                    "CK_Refunds_Amount",
                    "[Amount] > 0");

                table.HasCheckConstraint(
                    "CK_Refunds_ProcessedAt",
                    "[ProcessedAt] IS NULL OR [ProcessedAt] >= [CreatedAt]");
            });
            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.RefundNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.Status).IsRequired().HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.Reason).HasMaxLength(500);
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.ProcessedByUserId).HasMaxLength(128);
            builder.HasIndex(x => x.RefundNumber).IsUnique();

            // Indexes
            builder.HasIndex(x => x.ReturnId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.ProcessedByUserId);

            // Relationships
            builder.HasOne(x => x.Return)
                .WithMany(x => x.Refunds)
                .HasForeignKey(x => x.ReturnId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ProcessedByUser)
                .WithMany()
                .HasForeignKey(x => x.ProcessedByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}