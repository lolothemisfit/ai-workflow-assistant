using AIWorkflowAssistant.Api.DTOs;

namespace AIWorkflowAssistant.Api.Interfaces
{
    public interface IDocumentService
    {
        DocumentResponseDto ProcessDocument(DocumentRequestDto request);
    }
}