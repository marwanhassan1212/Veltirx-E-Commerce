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
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Slug).IsRequired().HasMaxLength(180);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.Property(x => x.ImageUrl).HasMaxLength(2048);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);


            //Indexes
            builder.HasIndex(x => x.Slug).IsUnique();
            builder.HasIndex(x => x.ParentCategoryId);

            //Default Values
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.DisplayOrder).HasDefaultValue(0);

            //Table
            builder.ToTable("Categories", table =>
            {
                table.HasCheckConstraint(
                    "CK_Categories_NoSelfParent",
                    "[ParentCategoryId] IS NULL OR [ParentCategoryId] <> [Id]"
                );
            });

            // Hierarchical Relationship
            builder.HasOne(x => x.ParentCategory)
                .WithMany(x => x.SubCategories)
                .HasForeignKey(x => x.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
