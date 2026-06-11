namespace roomy.Application.DTOs
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid OfficeId { get; set; }
        public string OfficeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
