using Microsoft.AspNetCore.Http;


    public class UploadFormDto
    {
        public IFormFile File { get; set; }
        public string? Title { get; set; }
    }

