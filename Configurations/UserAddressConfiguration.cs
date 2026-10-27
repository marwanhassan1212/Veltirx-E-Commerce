using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            // Table and Constraints
            builder.ToTable("UserAddresses", table =>
            {
                table.HasCheckConstraint(
                    "CK_UserAddresses_RecipientName",
                    "LEN(LTRIM(RTRIM([RecipientName]))) > 0");

                table.HasCheckConstraint(
                    "CK_UserAddresses_PhoneNumber",
                    "LEN(LTRIM(RTRIM([PhoneNumber]))) > 0");

                table.HasCheckConstraint(
                    "CK_UserAddresses_AddressLine1",
                    "LEN(LTRIM(RTRIM([AddressLine1]))) > 0");

                table.HasCheckConstraint(
                    "CK_UserAddresses_City",
                    "LEN(LTRIM(RTRIM([City]))) > 0");

                table.HasCheckConstraint(
                    "CK_UserAddresses_Governorate",
                    "LEN(LTRIM(RTRIM([Governorate]))) > 0");
            });

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.RecipientName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(30);
            builder.Property(x => x.AddressLine1).IsRequired().HasMaxLength(250);
            builder.Property(x => x.AddressLine2).HasMaxLength(250);
            builder.Property(x => x.Governorate).IsRequired().HasMaxLength(100);
            builder.Property(x => x.City).IsRequired().HasMaxLength(100);
            builder.Property(x => x.PostalCode).HasMaxLength(20);
            builder.Property(x => x.DeliveryInstructions).HasMaxLength(500);
            builder.Property(x => x.IsDefault).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(128);

            // Indexes
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => new { x.UserId, x.IsDefault });

            // Relationships
            builder.HasOne(x => x.User).WithMany(x => x.UserAddresses)
                .HasForeignKey(x => x.UserId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
