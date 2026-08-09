using AIWorkflowAssistant.Api.DTOs;

namespace AIWorkflowAssistant.Api.Interfaces
{
    public interface IDocumentService
    {
        Task<DocumentResponseDto> ProcessDocumentAsync(DocumentRequestDto request);
    }
}