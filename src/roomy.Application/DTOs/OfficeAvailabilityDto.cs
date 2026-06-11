namespace roomy.Application.DTOs
{
    public class OfficeAvailabilityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int TotalWorkspaces { get; set; }
        public int OccupiedWorkspaces { get; set; }
        public int AvailableWorkspaces { get; set; }
        public DateOnly Date { get; set; }
    }
}
