
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            // Table
            builder.ToTable("AuditLogs", table =>
            {
                table.HasCheckConstraint(
                    "CK_AuditLogs_Action",
                    "LEN(LTRIM(RTRIM([Action]))) > 0");

                table.HasCheckConstraint(
                    "CK_AuditLogs_EntityType",
                    "LEN(LTRIM(RTRIM([EntityType]))) > 0");

                table.HasCheckConstraint(
                    "CK_AuditLogs_EntityId",
                    "LEN(LTRIM(RTRIM([EntityId]))) > 0");
            });

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Action).IsRequired()
                .HasMaxLength(100);


            builder.Property(x => x.EntityType).IsRequired().HasMaxLength(100);
            builder.Property(x => x.EntityId).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Details).HasMaxLength(4000);
            builder.Property(x => x.IpAddress).HasMaxLength(45);
            builder.Property(x => x.UserAgent).HasMaxLength(1000);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UserId).HasMaxLength(128);

            // Indexes
            builder.HasIndex(x => x.CreatedAt);
            builder.HasIndex(x => new
            {
                x.EntityType,
                x.EntityId
            });

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => new
            {
                x.Action,
                x.CreatedAt
            });

            // Relationship
            builder.HasOne(x => x.User)
                .WithMany(x => x.AuditLogs)
                .HasForeignKey(x => x.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
