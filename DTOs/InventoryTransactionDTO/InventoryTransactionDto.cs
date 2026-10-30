using Veltrix.Enums;

namespace Veltrix.DTOs.InventoryTransactionDTO
{
    public class InventoryTransactionDto
    {
        public long Id { get; set; }

        public InventoryTransactionType TransactionType { get; set; }

        public int QuantityChange { get; set; }

        public int QuantityBefore { get; set; }

        public int QuantityAfter { get; set; }

        public string? Reason { get; set; }

        public DateTime CreatedAt { get; set; }

        public int InventoryId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? PerformedByUserId { get; set; }

        public string? PerformedByUserName { get; set; }

        public int? OrderId { get; set; }
    }
}
