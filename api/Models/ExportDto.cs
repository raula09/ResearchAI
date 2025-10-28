namespace ResearchCopilot.Api.Models
{
    public class ExportDto
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public List<string> Snippets { get; set; }
    }
}
