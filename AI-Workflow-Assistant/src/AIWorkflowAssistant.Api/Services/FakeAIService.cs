using AIWorkflowAssistant.Api.DTOs;
using AIWorkflowAssistant.Api.Interfaces;
using OpenAI.Chat;

namespace AIWorkflowAssistant.Api.Services;

public class FakeAIService : IAIService
{
    public Task<AiSummaryDto> GenerateSummaryAsync(string documentContent)
    {
        return Task.FromResult(new AiSummaryDto
        {
            Summary = "The meeting discussed project milestones, authentication, dashboard performance improvements, API documentation updates, and a deployment planned for next Friday.",
            Status = "Completed",
            ActionItems =
            [
                "Implement authentication",
                "Optimize dashboard",
                "Update API documentation",
                "Prepare deployment"
            ]
        });
    }

    public Task<SpreadsheetAnalysisDto> AnalyzeSpreadsheetAsync(
        SpreadsheetDataDto spreadsheet)
    {
        var issues = new List<SpreadsheetIssueDto>();

        for (var rowIndex = 0; rowIndex < spreadsheet.Rows.Count; rowIndex++)
        {
            var row = spreadsheet.Rows[rowIndex];

            foreach (var header in spreadsheet.Headers)
            {
                if (!row.TryGetValue(header, out var value) ||
                    string.IsNullOrWhiteSpace(value))
                {
                    issues.Add(new SpreadsheetIssueDto
                    {
                        RowNumber = rowIndex + 2,
                        ColumnName = header,
                        Issue = "Missing value",
                        SuggestedValue = null
                    });
                }
            }
        }

        return Task.FromResult(new SpreadsheetAnalysisDto
        {
            Summary = "Spreadsheet analysis complete.",
            Issues = issues,
            Recommendations=
            [
                "Review rows with missing values.",
                "Validate inconsistent data before generating the final spreadsheet."
            ]
        });
    }
}