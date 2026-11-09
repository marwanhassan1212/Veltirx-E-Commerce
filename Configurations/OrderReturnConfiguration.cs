using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class OrderReturnConfiguration
        : IEntityTypeConfiguration<OrderReturn>
    {
        public void Configure(EntityTypeBuilder<OrderReturn> builder)
        {

            // Table
            builder.ToTable("OrderReturns", table =>
            {
                table.HasCheckConstraint(
                    "CK_OrderReturns_ApprovedAt",
                    "[ApprovedAt] IS NULL OR [ApprovedAt] >= [RequestedAt]");

                table.HasCheckConstraint(
                    "CK_OrderReturns_ReceivedAt",
                    "[ReceivedAt] IS NULL OR [ReceivedAt] >= [RequestedAt]");

                table.HasCheckConstraint(
                    "CK_OrderReturns_CompletedAt",
                    "[CompletedAt] IS NULL OR [CompletedAt] >= [RequestedAt]");
            });

            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.ReturnNumber).IsRequired().HasMaxLength(50);


            // Properties
            builder.Property(x => x.Status).IsRequired().HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.Reason).IsRequired().HasMaxLength(500);
            builder.Property(x => x.CustomerNotes).HasMaxLength(1000);
            builder.Property(x => x.AdminNotes).HasMaxLength(1000);
            builder.Property(x => x.RequestedAt).IsRequired();
            builder.HasIndex(x => x.ReturnNumber).IsUnique();

            // Indexes
            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.Status);


            // Relationships
            builder.HasOne(x => x.Order)
                .WithMany(x => x.Returns)
                .HasForeignKey(x => x.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Items)
                .WithOne(x => x.Return)
                .HasForeignKey(x => x.ReturnId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Refunds)
                .WithOne(x => x.Return)
                .HasForeignKey(x => x.ReturnId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}