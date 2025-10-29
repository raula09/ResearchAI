using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchCopilot.Api.Repos;
using ResearchCopilot.Api.Utils;
using System.Threading.Tasks;
using ResearchCopilot.Api.Services;
namespace ResearchCopilot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly DocumentRepo _docs;
        private readonly ChatRepo _chat;
        public AnalyticsController(DocumentRepo docs, ChatRepo chat) { _docs = docs; _chat = chat; }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> UserStats()
        {
            var uid = JwtService.UserId(User);
            var docCount = await _docs.CountByUser(uid);
            var chatCount = await _chat.CountByUser(uid);
            return Ok(new { documents = docCount, questions = chatCount });
        }
    }
}
