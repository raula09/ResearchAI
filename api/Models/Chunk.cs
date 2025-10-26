namespace ResearchCopilot.Api.Models;
public class Chunk
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DocumentId { get; set; } = "";
    public string UserId { get; set; } = "";
    public string Text { get; set; } = "";
    public float[] Embedding { get; set; } = Array.Empty<float>();
    public int Order { get; set; }
}
