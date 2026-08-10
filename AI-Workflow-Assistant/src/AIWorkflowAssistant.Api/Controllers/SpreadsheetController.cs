using AIWorkflowAssistant.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AIWorkflowAssistant.Api.Models;

namespace AIWorkflowAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpreadsheetController : ControllerBase
{
    private readonly ISpreadsheetService _spreadsheetService;
    private readonly ISpreadsheetFileGenerator _fileGenerator;

    public SpreadsheetController(
        ISpreadsheetService spreadsheetService,
        ISpreadsheetFileGenerator fileGenerator)
    {
        _spreadsheetService =
            spreadsheetService;

        _fileGenerator =
            fileGenerator;
    }

    [HttpPost("analyze")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Analyze(
        [FromForm] SpreadsheetUploadRequest request)
    {
        if (request.File == null ||
            request.File.Length == 0)
        {
            return BadRequest(
                "A spreadsheet file is required.");
        }

        var spreadsheet =
            await _spreadsheetService
                .ExtractAsync(request.File);

        var analysis =
            await _spreadsheetService
                .ProcessAsync(request.File);

        var outputPath =
            await _fileGenerator.GenerateAsync(
                request.File.FileName,
                spreadsheet,
                analysis);

        var fileBytes =
            await System.IO.File.ReadAllBytesAsync(
                outputPath);

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Path.GetFileName(outputPath));
    }
}