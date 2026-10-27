using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            // Keys
            builder.HasKey(x => x.Id);

            //Properties
            builder.Property(x => x.ImageUrl).IsRequired().HasMaxLength(2048);
            builder.Property(x => x.AltText).HasMaxLength(300);
            builder.Property(x => x.IsPrimary).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.DisplayOrder).HasDefaultValue(0);
            builder.Property(x => x.CreatedAt).IsRequired();

            //Relations
            builder.HasOne(x => x.Product).WithMany(x => x.ProductImages)
                .HasForeignKey(x => x.ProductId).IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Tables
            builder.ToTable("ProductImages");
        }
    }
}
