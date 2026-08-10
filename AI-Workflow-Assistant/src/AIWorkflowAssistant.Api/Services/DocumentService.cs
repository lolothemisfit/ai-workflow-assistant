using AIWorkflowAssistant.Api.DTOs;
using AIWorkflowAssistant.Api.Interfaces;
using AIWorkflowAssistant.Api.Data;
using AIWorkflowAssistant.Api.Models;
using AIWorkflowAssistant.Api.Services.DocumentExtraction;
using AIWorkflowAssistant.Api.Services.FileGeneration;

namespace AIWorkflowAssistant.Api.Services;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IAIService _aiService;
    private readonly IEnumerable<IDocumentExtractor> _extractors;
    private readonly IDocumentFileGenerator _fileGenerator;

    public DocumentService(
        ApplicationDbContext context,
        IAIService aiService,
        IEnumerable<IDocumentExtractor> extractors,
        IDocumentFileGenerator fileGenerator)
    {
        _context = context;
        _aiService = aiService;
        _extractors = extractors;
        _fileGenerator = fileGenerator;
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

        string outputFilePath;

        if (fileExtension == ".pdf")
        {
            outputFilePath =
                await _fileGenerator.GeneratePdfAsync(
                    file.FileName,
                    aiResult);
        }
        else
        {
            outputFilePath =
                await _fileGenerator.GenerateDocxAsync(
                    file.FileName,
                    aiResult);
        }

        var document = new ProcessedDocument
        {
            OriginalFileName = file.FileName,
            FileType = fileExtension,
            OriginalContent = documentContent,
            AiSummary = aiResult.Summary,
            ActionItems = string.Join(
                Environment.NewLine,
                aiResult.ActionItems),
            OutputFilePath = outputFilePath,
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

    public async Task<(byte[] FileBytes, string ContentType, string FileName)> GetGeneratedFileAsync(
        Guid documentId)
    {
        var document = await _context.ProcessedDocuments
            .FindAsync(documentId);

        if (document == null)
        {
            throw new KeyNotFoundException(
                $"Processed document '{documentId}' was not found.");
        }

        if (string.IsNullOrWhiteSpace(document.OutputFilePath))
        {
            throw new InvalidOperationException(
                "No generated file exists for this document.");
        }

        if (!File.Exists(document.OutputFilePath))
        {
            throw new FileNotFoundException(
                "The generated file could not be found.",
                document.OutputFilePath);
        }

        var fileBytes = await File.ReadAllBytesAsync(
            document.OutputFilePath);

        var fileName = Path.GetFileName(
            document.OutputFilePath);

        var contentType = GetContentType(
            document.OutputFilePath);

        return (
            fileBytes,
            contentType,
            fileName
        );
    }

    private static string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath)
            .ToLowerInvariant();

        return extension switch
        {
            ".pdf" =>
                "application/pdf",

            ".docx" =>
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

            ".xlsx" =>
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

            ".csv" =>
                "text/csv",

            _ =>
                "application/octet-stream"
        };
    }
}