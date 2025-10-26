using MongoDB.Driver;
using ResearchCopilot.Api.Models;

namespace ResearchCopilot.Api.Repos;
public class MongoContext
{
    public IMongoDatabase Db { get; }
    public IMongoCollection<User> Users => Db.GetCollection<User>("users");
    public IMongoCollection<Document> Documents => Db.GetCollection<Document>("documents");
    public IMongoCollection<Chunk> Chunks => Db.GetCollection<Chunk>("chunks");
    public MongoContext()
    {
        var conn = Environment.GetEnvironmentVariable("MONGO_URI") ?? "mongodb://localhost:27017";
        var name = Environment.GetEnvironmentVariable("MONGO_DB") ?? "researchcopilot";
        var client = new MongoClient(conn);
        Db = client.GetDatabase(name);
    }
}
