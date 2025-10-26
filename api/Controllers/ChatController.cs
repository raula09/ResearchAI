using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchCopilot.Api.Models;
using ResearchCopilot.Api.Services;

namespace ResearchCopilot.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly RetrievalService _retrieval;
    public ChatController(RetrievalService retrieval) { _retrieval = retrieval; }

    [Authorize]
    [HttpPost("ask")]
    public async Task<ActionResult<QueryResponse>> Ask(QueryDto dto)
    {
        var uid = JwtService.UserId(User);
        var result = await _retrieval.Ask(uid, dto.Message);
        return new QueryResponse { Answer = result.answer, Snippets = result.snippets };
    }
}
