using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ResearchCopilot.Api.Models;

public class Document
{
    [BsonId] public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string UserId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
     public List<string> Tags { get; set; } = new();
}
