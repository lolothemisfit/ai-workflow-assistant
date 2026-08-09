using Microsoft.AspNetCore.Http;

namespace AIWorkflowAssistant.Api.DTOs;

public class DocumentRequestDto
{
    public IFormFile File { get; set; } = null!;
}