namespace AIWorkflowAssistant.Api.DTOs;

public class SpreadsheetDataDto
{
    public List<string> Headers { get; set; } = [];

    public List<Dictionary<string, string>> Rows { get; set; } = [];
}