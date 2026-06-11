namespace roomy.Application.DTOs
{
    public class CreateBookingDto
    {
        public Guid OfficeId { get; set; }
        public DateTime Date { get; set; }
        public Guid? UserId { get; set; }
    }
}
