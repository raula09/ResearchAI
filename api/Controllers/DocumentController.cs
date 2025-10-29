using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResearchCopilot.Api.Models;
using ResearchCopilot.Api.Repos;
using ResearchCopilot.Api.Services;
using ResearchCopilot.Api.Utils;
using System.Security.Claims;

namespace ResearchCopilot.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly DocumentService _service;
    private readonly DocumentRepo _repo;
    private readonly OpenRouterService _ai;

    public DocumentController(DocumentService service, DocumentRepo repo, OpenRouterService ai) { _service = service; _repo = repo; _ai = ai; }

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
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummaries()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        var docs = await _repo.ListDocs(userId);
        var combined = string.Join("\n\n", docs.Select(d => (d.Title ?? "") + "\n" + (d.Summary ?? "")));
        var summary = await _ai.GenerateAsync("Summarize these research documents into 3 bullet points with key insights.", combined);
        return Ok(new { summary });
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<ActionResult<List<Document>>> SearchByTag([FromQuery] string tag)
    {
        var uid = JwtService.UserId(User);
        if (string.IsNullOrWhiteSpace(tag)) return BadRequest();
        var docs = await _repo.SearchByTag(uid, tag);
        return docs;
    }

    [Authorize]
    [HttpPost("export")]
    public async Task<IActionResult> ExportAnswer([FromBody] ExportDto dto)
    {
        var uid = JwtService.UserId(User);
        var now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC";
        var title = "Untitled";
        var tags = new List<string>();
        if (!string.IsNullOrWhiteSpace(dto.DocumentId))
        {
            var d = await _repo.GetDoc(dto.DocumentId, uid);
            if (d != null) { title = d.Title; tags = d.Tags; }
        }
        Directory.CreateDirectory("exports");
        var path = Path.Combine("exports", $"{Guid.NewGuid()}.pdf");
        await System.IO.File.WriteAllTextAsync(path,
            "ResearchCopilot Export\n\n" +
            "Date: " + now + "\n" +
            "Title: " + title + "\n" +
            "Tags: " + string.Join(", ", tags) + "\n\n" +
            "Question:\n" + dto.Question + "\n\n" +
            "Answer:\n" + dto.Answer + "\n\n" +
            "Snippets:\n" + string.Join("\n", dto.Snippets));
        var bytes = await System.IO.File.ReadAllBytesAsync(path);
        return File(bytes, "application/pdf", "export.pdf");
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
