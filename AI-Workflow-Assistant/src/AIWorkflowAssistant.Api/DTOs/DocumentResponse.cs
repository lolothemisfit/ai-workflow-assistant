namespace AIWorkflowAssistant.Api.DTOs
{
    public class DocumentResponseDto
    {
        public string Summary { get; set; } = string.Empty;
        public List<string> ActionItems { get; set; } = [];
    }
}