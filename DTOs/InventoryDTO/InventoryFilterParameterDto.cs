
using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.InventoryDTO
{
    public class InventoryFilterDto
    {
        [StringLength(100)]
        public string? SearchTerm { get; set; }
        public string? StockStatus { get; set; }

        [Range(0, int.MaxValue)]
        public int? MinQuantity { get; set; }

        [Range(0, int.MaxValue)]
        public int? MaxQuantity { get; set; }
        public string SortBy { get; set; } = "RecentlyUpdated";

        public bool SortDescending { get; set; } = true;

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}
