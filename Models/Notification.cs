
using Veltrix.Enums;

namespace Veltrix.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;

        public Order? Order { get; set; }
        public int? OrderId { get; set; }
    }
}
