using ResearchCopilot.Api.Models;
using ResearchCopilot.Api.Repos;

namespace ResearchCopilot.Api.Services;
public class RetrievalService
{
    private readonly DocumentRepo _repo;
    private readonly ChatRepo _chat;
    private readonly OpenRouterService _ai;
    public RetrievalService(DocumentRepo repo, ChatRepo chat, OpenRouterService ai) { _repo = repo; _chat = chat; _ai = ai; }

    private static float CosSim(float[] a, float[] b)
    {
        double dot = 0; double na = 0; double nb = 0;
        for (int i = 0; i < a.Length; i++) { dot += a[i] * b[i]; na += a[i] * a[i]; nb += b[i] * b[i]; }
        if (na == 0 || nb == 0) return 0;
        return (float)(dot / (Math.Sqrt(na) * Math.Sqrt(nb)));
    }

    public async Task<(string answer, List<string> snippets)> Ask(string userId, string message)
    {
        var q = new float[] { 0f };
        var pool = await _repo.GetChunksByUser(userId);
        var ranked = pool.Select(c => new { c.Text, Score = CosSim(q, c.Embedding) }).OrderByDescending(x => x.Score).Take(8).ToList();
        var context = string.Join("\n\n", ranked.Select(x => x.Text));
        var sys = "Answer using the provided context only. Add concise references by quoting short phrases from the context.";
        var ans = await _ai.GenerateAsync(sys, "Question:\n" + message + "\nContext:\n" + context);
        await _chat.Insert(new ChatMessage { UserId = userId, Role = "user", Content = message });
        await _chat.Insert(new ChatMessage { UserId = userId, Role = "assistant", Content = ans });
        return (ans, ranked.Select(r => r.Text).ToList());
    }

    public async Task<string> FollowUp(string userId, string message, int history)
    {
        var recent = await _chat.RecentForUser(userId, history);
        recent.Reverse();
        var transcript = string.Join("\n\n", recent.Select(m => m.Role.ToUpper() + ": " + m.Content));
        var sys = "Continue the conversation. Be concise and accurate.";
        var ans = await _ai.GenerateAsync(sys, transcript + "\n\nUSER: " + message);
        await _chat.Insert(new ChatMessage { UserId = userId, Role = "user", Content = message });
        await _chat.Insert(new ChatMessage { UserId = userId, Role = "assistant", Content = ans });
        return ans;
    }
}
