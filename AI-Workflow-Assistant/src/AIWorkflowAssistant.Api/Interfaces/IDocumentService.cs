using AIWorkflowAssistant.Api.DTOs;

namespace AIWorkflowAssistant.Api.Interfaces;

public interface IDocumentService
{
    Task<DocumentResponseDto> ProcessDocumentAsync(
        DocumentRequestDto request);

    Task<(byte[] FileBytes, string ContentType, string FileName)> GetGeneratedFileAsync(
        Guid documentId);
}