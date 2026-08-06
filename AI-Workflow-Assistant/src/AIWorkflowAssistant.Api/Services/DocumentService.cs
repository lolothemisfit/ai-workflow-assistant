using AIWorkflowAssistant.Api.DTOs;
using AIWorkflowAssistant.Api.Interfaces;

namespace AIWorkflowAssistant.Api.Services;

public class DocumentService : IDocumentService
{
    public DocumentResponseDto ProcessDocument(DocumentRequestDto request)
    {
        return new DocumentResponseDto
        {
            Summary = "This is a placeholder summary.",
            ActionItems =
            [
                "Placeholder action item 1",
                "Placeholder action item 2"
            ]
        };
    }
}