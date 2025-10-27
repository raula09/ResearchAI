using MongoDB.Driver;
using ResearchCopilot.Api.Models;
using ResearchCopilot.Api.Database;

namespace ResearchCopilot.Api.Repos;
public class UserRepo
{
    private readonly MongoContext _ctx;
    public UserRepo(MongoContext ctx) { _ctx = ctx; }
    public Task<User?> FindByEmail(string email) => _ctx.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
    public async Task<User> Create(User u) { await _ctx.Users.InsertOneAsync(u); return u; }
}
