namespace AIWorkflowAssistant.Api.DTOs;

public class SpreadsheetIssueDto
{
    public int RowNumber { get; set; }

    public string ColumnName { get; set; } = string.Empty;

    public string Issue { get; set; } = string.Empty;

    public string CurrentValue { get; set; } = string.Empty;

    public string? SuggestedValue { get; set; }
}