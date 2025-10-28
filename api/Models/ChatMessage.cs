using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ResearchCopilot.Api.Models
{
    public class ChatMessage
    {
        [BsonId] public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        
    }
}
