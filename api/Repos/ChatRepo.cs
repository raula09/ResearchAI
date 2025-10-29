using MongoDB.Driver;
using ResearchCopilot.Api.Models;
using ResearchCopilot.Api.Database;


namespace ResearchCopilot.Api.Repos
{
    public class ChatRepo
    {
        private readonly MongoContext _ctx;
        public ChatRepo(MongoContext ctx) { _ctx = ctx; }
        public Task Insert(ChatMessage m) => _ctx.Chat.InsertOneAsync(m);
        public async Task<List<ChatMessage>> RecentForUser(string userId, int limit)
        {
            var f = Builders<ChatMessage>.Filter.Eq(x => x.UserId, userId);
            return await _ctx.Chat.Find(f).SortByDescending(x => x.CreatedAt).Limit(limit).ToListAsync();
        }
        public Task<long> CountByUser(string userId) => _ctx.Chat.CountDocumentsAsync(c => c.UserId == userId);
    }
}
