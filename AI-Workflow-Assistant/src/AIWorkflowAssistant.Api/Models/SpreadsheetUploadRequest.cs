namespace AIWorkflowAssistant.Api.Models;

public class SpreadsheetUploadRequest
{
    public IFormFile File { get; set; } = null!;
}