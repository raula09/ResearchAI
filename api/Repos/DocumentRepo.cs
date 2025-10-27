using MongoDB.Driver;
using ResearchCopilot.Api.Models;
using ResearchCopilot.Api.Database;

namespace ResearchCopilot.Api.Repos;
public class DocumentRepo
{
    private readonly MongoContext _ctx;
    public DocumentRepo(MongoContext ctx) { _ctx = ctx; }
    public Task InsertDoc(Document d) => _ctx.Documents.InsertOneAsync(d);
    public Task<Document?> GetDoc(string id, string userId) => _ctx.Documents.Find(x => x.Id == id && x.UserId == userId).FirstOrDefaultAsync();
    public Task<List<Document>> ListDocs(string userId) => _ctx.Documents.Find(x => x.UserId == userId).SortByDescending(x => x.CreatedAt).ToListAsync();
    public Task InsertChunks(IEnumerable<Chunk> chunks) => _ctx.Chunks.InsertManyAsync(chunks);
    public Task<List<Chunk>> GetChunksByUser(string userId, int limit = 2000) => _ctx.Chunks.Find(x => x.UserId == userId).Limit(limit).ToListAsync();
}
