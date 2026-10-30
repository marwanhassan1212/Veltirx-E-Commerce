using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.OrderDTO
{
    public class CancelOrderDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 3)]
        public string CancelReason { get; set; } = string.Empty;
    }
}
