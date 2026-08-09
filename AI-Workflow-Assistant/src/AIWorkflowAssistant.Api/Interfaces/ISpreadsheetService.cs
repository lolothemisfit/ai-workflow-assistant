using AIWorkflowAssistant.Api.DTOs;
using Microsoft.AspNetCore.Http;

namespace AIWorkflowAssistant.Api.Interfaces;

public interface ISpreadsheetService
{
    Task<SpreadsheetDataDto> ExtractAsync(IFormFile file);

    Task<SpreadsheetAnalysisDto> ProcessAsync(IFormFile file);
}