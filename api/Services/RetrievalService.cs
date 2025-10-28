using ResearchCopilot.Api.Models;
using ResearchCopilot.Api.Repos;

namespace ResearchCopilot.Api.Services
{
    public class RetrievalService
    {
        private readonly DocumentRepo _repo;
        private readonly OpenRouterService _ai;

        public RetrievalService(DocumentRepo repo, OpenRouterService ai)
        {
            _repo = repo;
            _ai = ai;
        }

        public async Task<(string answer, List<string> snippets)> Ask(string userId, string message)
        {
            var pool = await _repo.GetChunksByUser(userId);

           
            var topChunks = pool.Take(8).Select(c => c.Text).ToList();
            var context = string.Join("\n\n", topChunks);

            var sys = "Answer using only the provided context. If unsure, say 'Not enough information in the document.'";
            var prompt = $"Question:\n{message}\n\nContext:\n{context}";

            var ans = await _ai.GenerateAsync(sys, prompt);

            return (ans, topChunks);
        }
    }
}
