using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // Table
            builder.ToTable("Notifications");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Message).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.Type).IsRequired().HasConversion<string>().HasMaxLength(50);
            builder.Property(x => x.IsRead).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(128);

            // Indexes
            builder.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt });
            builder.HasIndex(x => x.OrderId);

            // Relationships
            builder.HasOne(x => x.User).WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Order).WithMany(x => x.Notifications)
                .HasForeignKey(x => x.OrderId).IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
 }

