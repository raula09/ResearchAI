using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchCopilot.Api.Repos;

namespace ResearchCopilot.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly DocumentRepo _repo;
    public SearchController(DocumentRepo repo) { _repo = repo; }

    [Authorize]
    [HttpGet("docs")]
    public async Task<ActionResult> Docs()
    {
        var uid = ResearchCopilot.Api.Services.JwtService.UserId(User);
        var list = await _repo.ListDocs(uid);
        return Ok(list.Select(x => new { x.Id, x.Title, x.Summary, x.CreatedAt }));
    }
}
