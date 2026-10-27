using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            // Keys
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(128);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            // RelationShips
            builder.HasOne(x => x.User).WithOne(x => x.Cart)
                .HasForeignKey<Cart>(x => x.UserId).IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Table
            builder.ToTable("Carts");
        }
    }
}
