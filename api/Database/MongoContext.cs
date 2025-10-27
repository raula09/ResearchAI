using MongoDB.Driver;
using ResearchCopilot.Api.Models;

namespace ResearchCopilot.Api.Database
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext()
        {
            var uri = Environment.GetEnvironmentVariable("MONGO_URI");
            var dbName = Environment.GetEnvironmentVariable("MONGO_DB");

            if (string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(dbName))
                throw new Exception("MongoDB environment variables not set.");

            var client = new MongoClient(uri);
            _database = client.GetDatabase(dbName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
        public IMongoCollection<Document> Documents => _database.GetCollection<Document>("documents");
        public IMongoCollection<Chunk> Chunks => _database.GetCollection<Chunk>("chunks");
    }
}
