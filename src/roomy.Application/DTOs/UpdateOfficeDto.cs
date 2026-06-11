namespace roomy.Application.DTOs
{
    public class UpdateOfficeDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int TotalWorkspaces { get; set; }
    }
}
