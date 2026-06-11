namespace roomy.Domain.Entities
{
    public class Office
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int TotalWorkspaces { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Booking> Bookings { get; } = [];

        public int GetOccupancyForDate(DateOnly date)
        {
            return Bookings
                .Where(b => DateOnly.FromDateTime(b.Date) == date && !b.CancelledAt.HasValue)
                .DistinctBy(b => b.UserId)
                .Count();
        }

        public bool CanBook(DateOnly date)
        {
            return GetOccupancyForDate(date) < TotalWorkspaces;
        }
    }
}
