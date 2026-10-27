using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Keys
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Slug).IsRequired().HasMaxLength(220);
            builder.Property(x => x.SKU).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ShortDescription).HasMaxLength(500);
            builder.Property(x => x.Description).HasMaxLength(4000);
            builder.Property(x => x.Price).HasPrecision(18, 2).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);

            // Indexes
            builder.HasIndex(x => x.Slug).IsUnique();
            builder.HasIndex(x => x.SKU).IsUnique();

            // Constraints
            builder.ToTable("Products", table =>
            table.HasCheckConstraint("Check_Product_Price", "[Price] >= 0"));

            // Default Values
            builder.Property(x => x.IsPublished).HasDefaultValue(false);

            //Relation Ships
            builder.HasOne(x => x.Category).WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
