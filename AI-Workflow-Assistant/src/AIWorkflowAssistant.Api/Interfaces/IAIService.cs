using AIWorkflowAssistant.Api.DTOs;

namespace AIWorkflowAssistant.Api.Interfaces;

public interface IAIService
{
    Task<AiSummaryDto> GenerateSummaryAsync(string documentContent);

    Task<SpreadsheetAnalysisDto> AnalyzeSpreadsheetAsync(
        SpreadsheetDataDto spreadsheet);
}