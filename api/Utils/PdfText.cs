using UglyToad.PdfPig;
using System.Text;

namespace ResearchCopilot.Api.Utils;
public static class PdfText
{
    public static string Extract(Stream pdfStream)
    {
        using var ms = new MemoryStream();
        pdfStream.CopyTo(ms);
        ms.Position = 0;
        var sb = new StringBuilder();
        using var doc = PdfDocument.Open(ms);
        foreach (var p in doc.GetPages()) sb.AppendLine(p.Text);
        return sb.ToString();
    }
}
