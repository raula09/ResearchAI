namespace ResearchCopilot.Api.Services;
public class ChunkingService
{
    public IEnumerable<(string chunk, int order)> Chunk(string text, int size = 1200, int overlap = 150)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var start = 0;
        var order = 0;
        while (start < words.Length)
        {
            var end = Math.Min(start + size, words.Length);
            var chunk = string.Join(" ", words[start..end]);
            yield return (chunk, order++);
            if (end == words.Length) break;
            start = end - overlap;
            if (start < 0) start = 0;
        }
    }
}
