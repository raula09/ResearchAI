using ResearchCopilot.Api.Models;
using ResearchCopilot.Api.Repos;

namespace ResearchCopilot.Api.Services;
public class RetrievalService
{
    private readonly DocumentRepo _repo;
    private readonly GeminiService _gemini;
    public RetrievalService(DocumentRepo repo, GeminiService gemini) { _repo = repo; _gemini = gemini; }

    private static float CosSim(float[] a, float[] b)
    {
        double dot = 0; double na = 0; double nb = 0;
        for (int i = 0; i < a.Length; i++) { dot += a[i] * b[i]; na += a[i] * a[i]; nb += b[i] * b[i]; }
        if (na == 0 || nb == 0) return 0;
        return (float)(dot / (Math.Sqrt(na) * Math.Sqrt(nb)));
    }

    public async Task<(string answer, List<string> snippets)> Ask(string userId, string message)
    {
        var q = await _gemini.EmbedAsync(message);
        var pool = await _repo.GetChunksByUser(userId);
        var ranked = pool.Select(c => new { c.Text, Score = CosSim(q, c.Embedding) }).OrderByDescending(x => x.Score).Take(8).ToList();
        var context = string.Join("\n\n", ranked.Select(x => x.Text));
        var sys = "Answer using the provided context only. Add concise references by quoting short phrases from the context.";
        var ans = await _gemini.GenerateAsync(sys, "Question:\n" + message + "\nContext:\n" + context);
        return (ans, ranked.Select(r => r.Text).ToList());
    }
}
