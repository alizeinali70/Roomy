namespace roomy.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OfficeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public User User { get; set; } = null!;
        public Office Office { get; set; } = null!;

        public bool IsActive => !CancelledAt.HasValue;
    }
}
