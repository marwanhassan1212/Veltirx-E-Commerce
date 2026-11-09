namespace Veltrix.Models
{
    public class OrderReturnItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string? ConditionNotes { get; set; }

        public OrderReturn Return { get; set; } = null!;
        public int ReturnId { get; set; }

        public OrderItem OrderItem { get; set; } = null!;
        public int OrderItemId { get; set; }
    }
}