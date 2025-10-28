using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ResearchCopilot.Api.Services
{
    public class OpenRouterService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public OpenRouterService(HttpClient http)
        {
            _http = http;
            _apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")
                      ?? throw new Exception("OPENROUTER_API_KEY not found in environment variables");
        }

        public async Task<string> GenerateAsync(string systemPrompt, string userMessage)
        {
            var url = "https://openrouter.ai/api/v1/chat/completions";

            var payload = new
            {
                model = "tngtech/deepseek-r1t2-chimera:free", // ✅ your model
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            // optional headers for OpenRouter ranking visibility
            _http.DefaultRequestHeaders.Add("HTTP-Referer", "https://researchcopilot.local");
            _http.DefaultRequestHeaders.Add("X-Title", "ResearchCopilot");

            var res = await _http.PostAsync(url, content);
            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception($"OpenRouter API error: {body}");

            using var doc = JsonDocument.Parse(body);
            var text = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return text ?? "";
        }
    }
}
