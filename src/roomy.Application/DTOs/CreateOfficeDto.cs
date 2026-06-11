namespace roomy.Application.DTOs
{
    public class CreateOfficeDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int TotalWorkspaces { get; set; }
    }
}
