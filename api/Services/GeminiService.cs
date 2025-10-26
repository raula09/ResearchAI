using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ResearchCopilot.Api.Services;
public class GeminiService
{
    private readonly HttpClient _http;
    private readonly string _key;
    public GeminiService(HttpClient http)
    {
        _http = http;
        _key = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";
    }

    public async Task<float[]> EmbedAsync(string text, int outputDim = 768)
    {
        var model = "text-embedding-004";
        var body = new
        {
            model,
            contents = new[] { new { role = "user", parts = new[] { new { text } } } },
            config = new { outputDimensionality = outputDim }
        };
        var json = JsonSerializer.Serialize(body);
        var res = await _http.PostAsync("https://generativelanguage.googleapis.com/v1beta/models/" + model + ":embedContent?key=" + _key, new StringContent(json, Encoding.UTF8, "application/json"));
        var s = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(s);
        var arr = doc.RootElement.GetProperty("embedding").GetProperty("values").EnumerateArray().Select(x => (float)x.GetDouble()).ToArray();
        return arr;
    }

    public async Task<string> GenerateAsync(string system, string user)
    {
        var model = "gemini-flash-latest";
        var body = new
        {
            contents = new object[]
            {
                new { role = "user", parts = new object[] { new { text = system + "\n" + user } } }
            }
        };
        var json = JsonSerializer.Serialize(body);
        var res = await _http.PostAsync("https://generativelanguage.googleapis.com/v1beta/models/" + model + ":generateContent?key=" + _key, new StringContent(json, Encoding.UTF8, "application/json"));
        var s = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(s);
        var text = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "";
        return text;
    }
}
