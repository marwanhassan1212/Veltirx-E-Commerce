using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Veltrix.Models;

namespace Veltrix.Configurations
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {

            // Table and Constraints 
            builder.ToTable("Inventories", table =>
            table.HasCheckConstraint(
                "Check_Inventory_StockQuantity", "[StockQuantity] >= 0"
                ));

            // Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.StockQuantity).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.UpdatedAt).IsRequired();

            //RelationShips
            builder.HasOne(x => x.Product).WithOne(x => x.Inventory)
                .HasForeignKey<Inventory>(x => x.ProductId).IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
