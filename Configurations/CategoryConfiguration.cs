using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            //Key
            builder.HasKey(x => x.Id);

            //Properties
            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Slug).IsRequired().HasMaxLength(180);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.Property(x => x.ImageUrl).HasMaxLength(2048);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);


            //Indexes
            builder.HasIndex(x => x.Slug).IsUnique();

            //Default Values
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.DisplayOrder).HasDefaultValue(0);

            //Table
            builder.ToTable("Categories");

        }
    }
}
