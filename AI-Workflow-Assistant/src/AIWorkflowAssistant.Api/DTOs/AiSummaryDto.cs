namespace AIWorkflowAssistant.Api.DTOs;

public class AiSummaryDto
{
    public string Summary { get; set; } = string.Empty;

    public List<string> ActionItems { get; set; } = [];

    public string Status { get; set; } = string.Empty;
}