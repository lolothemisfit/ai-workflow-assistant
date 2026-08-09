using AIWorkflowAssistant.Api.Interfaces;
using AIWorkflowAssistant.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIWorkflowAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpreadsheetController : ControllerBase
{
    private readonly ISpreadsheetService _spreadsheetService;

    public SpreadsheetController(ISpreadsheetService spreadsheetService)
    {
        _spreadsheetService = spreadsheetService;
    }

    [HttpPost("analyze")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Analyze(
        [FromForm] SpreadsheetUploadRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest("A spreadsheet file is required.");
        }

        var result = await _spreadsheetService.ProcessAsync(request.File);

        return Ok(result);
    }
}