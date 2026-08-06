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
        public ActionResult<DocumentResponseDto> ProcessDocument(DocumentRequestDto request)
        {
            var response = new DocumentResponseDto
            {
                Summary = "This is a summary of the document.",
                ActionItems =
                [
                    "Action Item 1",
                    "Action Item 2",
                    "Action Item 3"
                ]
            };
            return Ok(response);

        }
    }
}