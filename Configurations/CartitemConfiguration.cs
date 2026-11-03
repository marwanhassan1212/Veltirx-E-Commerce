using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class CartitemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            // Keys
            builder.HasKey(x => x.Id);

            // Table and Constraints
            builder.ToTable("CartItems", table =>
            {
                table.HasCheckConstraint(
                    "Check_Quantity_CartItems",
                    "[Quantity] > 0");
            });

            // Properties
            builder.Property(x => x.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(x => x.AddedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);

            // Relationships
            builder.HasOne(x => x.Cart)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.CartId)
                .IsRequired();


            builder.HasOne(x => x.ProductVariant)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.ProductVariantId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => new
            {
                x.CartId,
                x.ProductVariantId
            }).IsUnique();
        }
    }
}