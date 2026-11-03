namespace Veltrix.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public int StockQuantity { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
        public ProductVariant ProductVariant { get; set; } = null!;
        public int ProductVariantId { get; set; }
    }
}
