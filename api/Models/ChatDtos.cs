namespace ResearchCopilot.Api.Models;
public class QueryDto { public string Message { get; set; } = ""; }
public class QueryResponse { public string Answer { get; set; } = ""; public List<string> Snippets { get; set; } = new(); }
public class UploadTextDto { public string Title { get; set; } = ""; public string Text { get; set; } = ""; }
