namespace roomy.Application.DTOs
{
    public class OfficeAvailabilityRangeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int TotalWorkspaces { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public IReadOnlyList<OfficeAvailabilityEntryDto> Entries { get; set; } = [];
    }
}
