
namespace Veltrix.DTOs.OrderReturnDTO
{
    public class OrderReturnItemDto
    {
        public int Id { get; set; }
        public int ReturnId { get; set; }
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public string? ConditionNotes { get; set; }
    }
}
