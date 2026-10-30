using Veltrix.Enums;

namespace Veltrix.Models
{
    public class InventoryTransaction
    {
        public long Id { get; set; }
        public InventoryTransactionType TransactionType { get; set; }
        public int QuantityChange { get; set; }
        public int QuantityBefore { get; set; }
        public int QuantityAfter { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }


        public Inventory Inventory { get; set; } = null!;
        public int InventoryId { get; set; }
        public ApplicationUser? PerformedByUser { get; set; }
        public string? PerformedByUserId { get; set; }

        public Order? Order { get; set; }
        public int? OrderId { get; set; }

    }
}
