using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
        {
            // Table and Constraints
            builder.ToTable("OrderStatusHistory", table =>
            {
                table.HasCheckConstraint(
                    "CK_OrderStatusHistory_StatusChanged",
                    "[PreviousStatus] IS NULL OR [PreviousStatus] <> [NewStatus]");
            });

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.PreviousStatus).HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.NewStatus).IsRequired().HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.Note).HasMaxLength(1000);
            builder.Property(x => x.ChangedAt).IsRequired();
            builder.Property(x => x.OrderId).IsRequired();

            // Indexes
            builder.HasIndex(x => new { x.OrderId, x.ChangedAt });
            builder.HasIndex(x => x.ChangedByUserId);

            // Relationships
            builder.HasOne(x => x.Order).WithMany(x => x.StatusHistory).HasForeignKey(x => x.OrderId)
                .IsRequired().OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.ChangedByUser).WithMany(x => x.StatusHistory)
                .HasForeignKey(x => x.ChangedByUserId).IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
