using AIWorkflowAssistant.Api.DTOs;
using AIWorkflowAssistant.Api.Interfaces;
using AIWorkflowAssistant.Api.Data;
using AIWorkflowAssistant.Api.Models;
using AIWorkflowAssistant.Api.Services.DocumentExtraction;

namespace AIWorkflowAssistant.Api.Services;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IAIService _aiService;
    private readonly IEnumerable<IDocumentExtractor> _extractors;

    public DocumentService(
        ApplicationDbContext context,
        IAIService aiService,
        IEnumerable<IDocumentExtractor> extractors)
    {
        _context = context;
        _aiService = aiService;
        _extractors = extractors;
    }

    public async Task<DocumentResponseDto> ProcessDocumentAsync(
        DocumentRequestDto request)
    {
        var file = request.File;

        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("A document file is required.");
        }

        var fileExtension = Path.GetExtension(file.FileName)
            .ToLowerInvariant();

        if (fileExtension != ".pdf" && fileExtension != ".docx")
        {
            throw new ArgumentException(
                "Only PDF and DOCX files are supported.");
        }

        var documentContent = await ExtractTextAsync(
            file,
            fileExtension);

        var aiResult = await _aiService.GenerateSummaryAsync(
            documentContent);

        var document = new ProcessedDocument
        {
            OriginalFileName = file.FileName,
            FileType = fileExtension,
            OriginalContent = documentContent,
            AiSummary = aiResult.Summary,
            ActionItems = string.Join(
                Environment.NewLine,
                aiResult.ActionItems),
            Status = aiResult.Status,
            ProcessedAt = DateTime.UtcNow
        };

        _context.ProcessedDocuments.Add(document);

        await _context.SaveChangesAsync();

        return new DocumentResponseDto
        {
            DocumentId = document.Id,
            Status = document.Status,
            Summary = document.AiSummary,
            ActionItems = aiResult.ActionItems,
            OutputFilePath = document.OutputFilePath
        };
    }

    private async Task<string> ExtractTextAsync(
        IFormFile file,
        string fileExtension)
    {

        var extractor = _extractors.FirstOrDefault(
            e => e.CanHandle(fileExtension));

        if (extractor == null)
        {
            throw new InvalidOperationException(
                $"No extractor found for file type: {fileExtension}");
        }

        await using var stream = file.OpenReadStream();
        
        return await extractor.ExtractTextAsync(stream);
    }
}