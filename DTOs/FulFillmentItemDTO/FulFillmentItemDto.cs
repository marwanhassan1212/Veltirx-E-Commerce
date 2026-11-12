
namespace Veltrix.DTOs.OrderFulfillmentDTO
{
    public class FulfillmentItemDto
    {
        public int Id { get; set; }
        public int FulfillmentId { get; set; }
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
    }
}
