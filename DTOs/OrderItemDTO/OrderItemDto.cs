namespace Veltrix.DTOs.OrderItemDTO
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public int ProductVariantId { get; set; }
        public string? VariantName { get; set; }
        public string? VariantOptionsSnapshot { get; set; }
        public int CancelledQuantity { get; set; }
        public int FulfilledQuantity { get; set; }
    }
}
