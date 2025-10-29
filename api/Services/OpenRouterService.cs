using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ResearchCopilot.Api.Services
{
    public class OpenRouterService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string _model;

        public OpenRouterService(HttpClient http)
        {
            _http = http;
            _apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY") ?? throw new Exception("OPENROUTER_API_KEY not found in environment variables");
            _model = Environment.GetEnvironmentVariable("OPENROUTER_MODEL") ?? "qwen/qwen-2.5-7b-instruct:free";
        }

        public async Task<string> GenerateAsync(string systemPrompt, string userMessage)
        {
            var url = "https://openrouter.ai/api/v1/chat/completions";
            var payload = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage }
                }
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            var res = await _http.PostAsync(url, content);
            var body = await res.Content.ReadAsStringAsync();
            if (!res.IsSuccessStatusCode) throw new Exception($"OpenRouter API error: {body}");
            using var doc = JsonDocument.Parse(body);
            var text = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            return text ?? "";
        }

        public async Task<List<string>> TagsAsync(string title, string summary)
        {
            var sys = "Return only a JSON array of 3 to 6 concise tags. No prose.";
            var msg = "Title:\n" + title + "\nSummary:\n" + summary + "\nTags:";
            var raw = await GenerateAsync(sys, msg);
            try
            {
                var jsonStart = raw.IndexOf('[');
                var jsonEnd = raw.LastIndexOf(']');
                if (jsonStart >= 0 && jsonEnd > jsonStart) raw = raw.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var arr = JsonSerializer.Deserialize<List<string>>(raw) ?? new();
                return arr.Select(t => t.Trim()).Where(t => t.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            }
            catch
            {
                var csv = raw.Replace("\n", ",").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
                return csv.Take(6).Select(s => s.Trim()).Where(s => s.Length > 0).ToList();
            }
        }
    }
}
