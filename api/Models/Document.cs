namespace ResearchCopilot.Api.Models;
public class Document
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
