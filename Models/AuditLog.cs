namespace Veltrix.Models
{
    public class AuditLog
    {
        public long Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
        public ApplicationUser? User { get; set; }
        public string? UserId { get; set; }


    }
}
