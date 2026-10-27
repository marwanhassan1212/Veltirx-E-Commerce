using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            // Table
            builder.ToTable("ProductReviews", table =>
            {
                table.HasCheckConstraint(
                    "CK_ProductReviews_Rating",
                    "[Rating] >= 1 AND [Rating] <= 5");
            });

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Rating).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(200);
            builder.Property(x => x.Comment).HasMaxLength(2000);
            builder.Property(x => x.IsApproved).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(128);
            builder.Property(x => x.OrderItemId).IsRequired();

            // Indexes
            builder.HasIndex(x => new
            {
                x.ProductId,
                x.IsApproved,
                x.CreatedAt
            });

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.OrderItemId).IsUnique();


            // RelationShips
            builder.HasOne(x => x.Product).WithMany(x => x.ProductReviews)
                .HasForeignKey(x => x.ProductId).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User).WithMany(x => x.ProductReviews)
                    .HasForeignKey(x => x.UserId).IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OrderItem).WithMany(x => x.ProductReviews)
               .HasForeignKey(x => x.OrderItemId).IsRequired()
               .OnDelete(DeleteBehavior.Restrict);




        }
    }
}

