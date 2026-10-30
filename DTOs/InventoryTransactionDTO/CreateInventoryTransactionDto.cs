using System.ComponentModel.DataAnnotations;
using Veltrix.Enums;

namespace Veltrix.DTOs.InventoryTransactionDTO
{
    public class CreateInventoryTransactionDto
    {
        [Required]
        public InventoryTransactionType TransactionType { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string Reason { get; set; } = string.Empty;
    }
}
