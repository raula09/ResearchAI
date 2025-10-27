using Microsoft.AspNetCore.Http;

namespace ResearchCopilot.Api.Models
{
    public class UploadFormDto
    {
        public IFormFile File { get; set; }
        public string Title { get; set; }
    }
}
