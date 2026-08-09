namespace AIWorkflowAssistant.Api.DTOs;

public class SpreadsheetAnalysisDto
{
    public string Summary { get; set; } = string.Empty;

    public List<SpreadsheetIssueDto> Issues { get; set; } = [];

    public string Status { get; set; } = "Pending";

    public List<string> Recommendations { get; set; } = [];
}