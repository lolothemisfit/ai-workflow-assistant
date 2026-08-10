using AIWorkflowAssistant.Api.DTOs;

namespace AIWorkflowAssistant.Api.Services.FileGeneration;

public interface IDocumentFileGenerator
{
    Task<string> GenerateDocxAsync(
        string originalFileName,
        AiSummaryDto aiResult);

    Task<string> GeneratePdfAsync(
        string originalFileName,
        AiSummaryDto aiResult);
}