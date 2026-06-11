namespace roomy.Application.DTOs
{
    public class OfficeAvailabilityEntryDto
    {
        public DateOnly Date { get; set; }
        public int OccupiedWorkspaces { get; set; }
        public int AvailableWorkspaces { get; set; }
    }
}
