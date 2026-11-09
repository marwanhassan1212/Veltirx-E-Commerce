namespace Veltrix.Models
{
    public class CodReconciliationItem
    {
        public int Id { get; set; }

        public decimal ExpectedAmount { get; set; }
        public decimal CollectedAmount { get; set; }
        public decimal DifferenceAmount { get; set; }

        public CodReconciliation Reconciliation { get; set; } = null!;
        public int ReconciliationId { get; set; }

        public Order Order { get; set; } = null!;
        public int OrderId { get; set; }
    }
}