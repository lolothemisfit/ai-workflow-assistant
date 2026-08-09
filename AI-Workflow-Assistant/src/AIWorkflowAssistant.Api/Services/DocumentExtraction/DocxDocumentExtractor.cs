using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AIWorkflowAssistant.Api.Services.DocumentExtraction;

public class DocxDocumentExtractor : IDocumentExtractor
{
    public bool CanHandle(string fileExtension)
    {
        return fileExtension.Equals(
            ".docx",
            StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string> ExtractTextAsync(Stream fileStream)
    {
        using var memoryStream = new MemoryStream();

        await fileStream.CopyToAsync(memoryStream);

        memoryStream.Position = 0;

        using var document = WordprocessingDocument.Open(
            memoryStream,
            false);

        var body = document.MainDocumentPart?.Document.Body;

        if (body == null)
        {
            return string.Empty;
        }

        return string.Join(
            Environment.NewLine,
            body.Descendants<Text>()
                .Select(text => text.Text));
    }
}