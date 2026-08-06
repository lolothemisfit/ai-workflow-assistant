namespace AIWorkflowAssistant.Api.DTOs
{
    public class DocumentResponseDto
    {
        public Guid DocumentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string? OutputFilePath { get; set; }
        public List<string> ActionItems { get; set; } = [];
    }
}