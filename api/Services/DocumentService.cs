using ResearchCopilot.Api.Models;
using ResearchCopilot.Api.Repos;

namespace ResearchCopilot.Api.Services
{
    public class DocumentService
    {
        private readonly DocumentRepo _docs;
        private readonly ChunkingService _chunker;
        private readonly OpenRouterService _ai;

        public DocumentService(DocumentRepo docs, ChunkingService chunker, OpenRouterService ai)
        {
            _docs = docs;
            _chunker = chunker;
            _ai = ai;
        }

        public async Task<Document> IngestRaw(string userId, string title, string text)
        {
            var doc = new Document { UserId = userId, Title = title };
            await _docs.InsertDoc(doc);

            var chunks = new List<Chunk>();

            foreach (var c in _chunker.Chunk(text))
            {
                // Temporary fix: no embeddings
                chunks.Add(new Chunk
                {
                    DocumentId = doc.Id,
                    UserId = userId,
                    Text = c.chunk,
                    Embedding = new float[0],
                    Order = c.order
                });
            }

            var joined = string.Join("\n\n", chunks.Take(6).Select(x => x.Text));
            var summary = await _ai.GenerateAsync(
                "Summarize clearly with bullet points and headings.",
                joined
            );

            doc.Summary = summary;

            await _docs.InsertChunks(chunks);
            return doc;
        }
    }
}
