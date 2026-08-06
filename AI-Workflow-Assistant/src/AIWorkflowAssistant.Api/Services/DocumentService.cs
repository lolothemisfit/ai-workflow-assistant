using AIWorkflowAssistant.Api.DTOs;
using AIWorkflowAssistant.Api.Interfaces;
using AIWorkflowAssistant.Api.Data;
using AIWorkflowAssistant.Api.Models;

namespace AIWorkflowAssistant.Api.Services;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;

    public DocumentService(ApplicationDbContext context)
    {
        _context = context;
    }
    public DocumentResponseDto ProcessDocument(DocumentRequestDto request)
    {
        var document = new ProcessedDocument
        {
            OriginalFileName = request.DocumentName,
            FileType = request.FileType,
            OriginalContent = request.DocumentContent,
            AiSummary = "Pending",
            Status = "Pending"
        };

        _context.ProcessedDocuments.Add(document);

        _context.SaveChanges();

        return new DocumentResponseDto
        {
            DocumentId = document.Id,
            Status = document.Status,
            Summary = document.AiSummary,
            OutputFilePath = document.OutputFilePath,
            ActionItems = []
        };
    }
}