namespace Veltrix.Models
{
    public class FulfillmentItem
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public OrderFulfillment Fulfillment { get; set; } = null!;
        public int FulfillmentId { get; set; }

        public OrderItem OrderItem { get; set; } = null!;
        public int OrderItemId { get; set; }
    }
}