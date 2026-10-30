using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.InventoryDTO
{
    public class UpdateInventoryDto
    {
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string Reason { get; set; } = string.Empty;
    }
}
