using AIWorkflowAssistant.Api.DTOs;

namespace AIWorkflowAssistant.Api.Interfaces;

public interface ISpreadsheetFileGenerator
{
    Task<string> GenerateAsync(
        string originalFileName,
        SpreadsheetDataDto spreadsheet,
        SpreadsheetAnalysisDto analysis);
}