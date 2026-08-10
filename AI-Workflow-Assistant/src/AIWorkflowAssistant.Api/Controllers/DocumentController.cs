using AIWorkflowAssistant.Api.DTOs;
using AIWorkflowAssistant.Api.Interfaces;
using AIWorkflowAssistant.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIWorkflowAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly ApplicationDbContext _context;

    public DocumentController(
        IDocumentService documentService,
        ApplicationDbContext context)
    {
        _documentService = documentService;
        _context = context;
    }

    [HttpPost("process")]
    public async Task<ActionResult<DocumentResponseDto>> ProcessDocument(
        [FromForm] DocumentRequestDto request)
    {
        var response = await _documentService.ProcessDocumentAsync(request);

        return Ok(response);
    }

    [HttpGet("{id}/file")]
    public async Task<IActionResult> GetGeneratedFile(Guid id)
    {
        var document = await _context.ProcessedDocuments
            .FirstOrDefaultAsync(d => d.Id == id);

        if (document == null)
        {
            return NotFound("Document not found.");
        }

        if (string.IsNullOrWhiteSpace(document.OutputFilePath))
        {
            return NotFound("No generated file exists for this document.");
        }

        if (!System.IO.File.Exists(document.OutputFilePath))
        {
            return NotFound("Generated file could not be found.");
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(
            document.OutputFilePath);

        var contentType = GetContentType(document.OutputFilePath);

        var fileName = Path.GetFileName(document.OutputFilePath);

        return File(
            fileBytes,
            contentType,
            fileName);
    }

    private static string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath)
            .ToLowerInvariant();

        return extension switch
        {
            ".pdf" => "application/pdf",

            ".docx" =>
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

            ".xlsx" =>
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

            _ => "application/octet-stream"
        };
    }
}