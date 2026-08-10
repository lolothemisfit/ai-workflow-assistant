namespace AIWorkflowAssistant.Api.Services.DocumentExtraction;

public interface IDocumentExtractor
{
    bool CanHandle(string fileExtension);

    Task<string> ExtractTextAsync(Stream fileStream);
}