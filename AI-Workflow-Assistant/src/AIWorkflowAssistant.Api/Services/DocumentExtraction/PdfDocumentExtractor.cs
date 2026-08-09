using UglyToad.PdfPig;

namespace AIWorkflowAssistant.Api.Services.DocumentExtraction;

public class PdfDocumentExtractor : IDocumentExtractor
{
    public bool CanHandle(string fileExtension)
    {
        return fileExtension.Equals(
            ".pdf",
            StringComparison.OrdinalIgnoreCase);
    }

    public Task<string> ExtractTextAsync(Stream fileStream)
    {
        using var memoryStream = new MemoryStream();

        fileStream.CopyTo(memoryStream);

        memoryStream.Position = 0;

        using var document = PdfDocument.Open(memoryStream);

        var pages = document.GetPages();

        var text = string.Join(
            Environment.NewLine,
            pages.Select(page => page.Text));

        return Task.FromResult(text);
    }
}