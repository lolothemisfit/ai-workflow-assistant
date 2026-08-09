using AIWorkflowAssistant.Api.DTOs;
using AIWorkflowAssistant.Api.Interfaces;
using OpenAI.Chat;

namespace AIWorkflowAssistant.Api.Services;

    public class AIService : IAIService
    {
        private readonly IConfiguration _configuration;

        public AIService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AiSummaryDto> GenerateSummaryAsync(string documentContent)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            var model = _configuration["OpenAI:Model"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("OpenAI API key is missing.");
            }

            var chatClient = new ChatClient(
                model: model!,
                apiKey: apiKey
            );

            var prompt = $"""
            You are an AI workflow assistant.

            Summarize the following document in a concise manner.

            Then provide a list of action items.

            Document:

            {documentContent}
            """;

            var response = await chatClient.CompleteChatAsync(prompt);

            var summary = response.Value.Content[0].Text;

            return new AiSummaryDto
            {
                Summary = summary,
                ActionItems = [],
                Status = "Completed"
            };
        }

        public Task<SpreadsheetAnalysisDto> AnalyzeSpreadsheetAsync(
            SpreadsheetDataDto spreadsheet)
        {
            throw new NotImplementedException(
                "Spreadsheet AI analysis will be implemented with Azure OpenAI.");
        }
    }
