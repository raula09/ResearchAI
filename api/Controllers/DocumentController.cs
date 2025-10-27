using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchCopilot.Api.Models;
using ResearchCopilot.Api.Repos;
using ResearchCopilot.Api.Services;
using ResearchCopilot.Api.Utils;

namespace ResearchCopilot.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly DocumentService _service;
    private readonly DocumentRepo _repo;
    public DocumentController(DocumentService service, DocumentRepo repo) { _service = service; _repo = repo; }
[Authorize]
[HttpPost("upload")]
[Consumes("multipart/form-data")]
public async Task<ActionResult<Document>> Upload([FromForm] UploadFormDto dto)
{
    var uid = JwtService.UserId(User);
    if (dto.File == null || dto.File.Length == 0) return BadRequest("No file uploaded.");

    using var s = dto.File.OpenReadStream();
    var text = PdfText.Extract(s);
    var doc = await _service.IngestRaw(uid, dto.Title, text);
    return doc;
}

    [Authorize]
    [HttpPost("text")]
    public async Task<ActionResult<Document>> UploadText(UploadTextDto dto, [FromServices] DocumentService ds)
    {
        var uid = JwtService.UserId(User);
        var doc = await ds.IngestRaw(uid, dto.Title, dto.Text);
        return doc;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<Document>>> List([FromServices] DocumentRepo repo)
    {
        var uid = JwtService.UserId(User);
        return await repo.ListDocs(uid);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Document>> Get(string id)
    {
        var uid = JwtService.UserId(User);
        var d = await _repo.GetDoc(id, uid);
        if (d == null) return NotFound();
        return d;
    }
}
