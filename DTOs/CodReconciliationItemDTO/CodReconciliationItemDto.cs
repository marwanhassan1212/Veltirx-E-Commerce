
namespace Veltrix.DTOs.CodReconciliationDTO
{
    public class CodReconciliationItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal ExpectedAmount { get; set; }
        public decimal CollectedAmount { get; set; }
        public decimal DifferenceAmount { get; set; }
    }
}
