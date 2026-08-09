namespace AIWorkflowAssistant.Api.Models;

public class ProcessedDocument
{
    public Guid Id { get; set; }

    public string OriginalFileName { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public string OriginalContent { get; set; } = string.Empty;

    public string AiSummary { get; set; } = string.Empty;

    public string ActionItems { get; set; } = "[]";

    public string? OutputFilePath { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessedAt { get; set; }
}