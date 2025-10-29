using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchCopilot.Api.Models;
using ResearchCopilot.Api.Services;
using ResearchCopilot.Api.Utils;

namespace ResearchCopilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly RetrievalService _retrieval;
    public ChatController(RetrievalService retrieval) { _retrieval = retrieval; }

    [Authorize]
    [HttpPost("ask")]
    public async Task<ActionResult<object>> Ask(QueryDto dto)
    {
        var uid = JwtService.UserId(User);
        var (answer, snippets) = await _retrieval.Ask(uid, dto.Message);
        return Ok(new { answer, snippets });
    }

    [Authorize]
    [HttpPost("followup")]
    public async Task<ActionResult<object>> FollowUp(FollowUpDto dto)
    {
        var uid = JwtService.UserId(User);
        var answer = await _retrieval.FollowUp(uid, dto.Message, Math.Clamp(dto.HistoryLimit, 2, 20));
        return Ok(new { answer });
    }
}
