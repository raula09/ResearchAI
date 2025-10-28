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

    public DocumentController(DocumentService service, DocumentRepo repo, OpenRouterService ai)
    {
        _service = service;
        _repo = repo;
        _ai = ai;
    }

    [Authorize]
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Document>> Upload([FromForm] UploadFormDto dto)
    {
        var uid = JwtService.UserId(User);
        if (dto.File == null || dto.File.Length == 0)
            return BadRequest("No file uploaded.");

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
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var docs = await _repo.ListDocs(userId);

        if (docs.Count == 0)
            return Ok(new { summary = "No documents found for this user." });

        var combined = string.Join("\n\n", docs.Select(d => d.Summary ?? d.Title));

        var summary = await _ai.GenerateAsync(
            "Summarize these research documents into 3 concise bullet points with clear insights.",
            combined
        );

        return Ok(new { summary });
    }

   
    [Authorize]
    [HttpPost("export")]
    public async Task<IActionResult> ExportAnswer([FromBody] ExportDto dto)
    {
        var pdfPath = Path.Combine("exports", $"{Guid.NewGuid()}.pdf");
        Directory.CreateDirectory("exports");

        using var writer = new StreamWriter(pdfPath);
        await writer.WriteLineAsync($"Question: {dto.Question}\n");
        await writer.WriteLineAsync($"Answer: {dto.Answer}\n\nSnippets:\n{string.Join("\n", dto.Snippets)}");

        var bytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
        return File(bytes, "application/pdf", "export.pdf");
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<Document>>> List()
    {
        var uid = JwtService.UserId(User);
        return await _repo.ListDocs(uid);
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
