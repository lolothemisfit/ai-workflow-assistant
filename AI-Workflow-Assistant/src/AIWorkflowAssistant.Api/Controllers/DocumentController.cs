using AIWorkflowAssistant.Api.DTOs;
using AIWorkflowAssistant.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIWorkflowAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("process")]
        public async Task<ActionResult<DocumentResponseDto>> ProcessDocument([FromForm] DocumentRequestDto request)
        {
            var response = await _documentService.ProcessDocumentAsync(request);

            return Ok(response);

        }
    }
}