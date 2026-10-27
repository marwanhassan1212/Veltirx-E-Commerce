namespace Veltrix.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public int StockQuantity { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
