namespace Veltrix.DTOs.InventoryDTO
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
