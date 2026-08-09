using System.Text.Json;
using AIWorkflowAssistant.Api.DTOs;
using AIWorkflowAssistant.Api.Interfaces;
using Azure.AI.Projects;
using Azure.Identity;
using OpenAI.Chat;

namespace AIWorkflowAssistant.Api.Services;

public class AIService : IAIService
{
    private readonly ChatClient _chatClient;

    public AIService(IConfiguration configuration)
    {
        var endpoint = configuration["AzureOpenAI:Endpoint"];
        var deploymentName = configuration["AzureOpenAI:DeploymentName"];

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new InvalidOperationException(
                "Azure AI Foundry project endpoint is missing.");
        }

        if (string.IsNullOrWhiteSpace(deploymentName))
        {
            throw new InvalidOperationException(
                "Azure AI deployment name is missing.");
        }

        var projectClient = new AIProjectClient(
            new Uri(endpoint),
            new AzureCliCredential());

        _chatClient = projectClient
        .ProjectOpenAIClient
        .GetChatClient(deploymentName);
    }

    public async Task<AiSummaryDto> GenerateSummaryAsync(
        string documentContent)
    {
        if (string.IsNullOrWhiteSpace(documentContent))
        {
            throw new ArgumentException(
                "Document content cannot be empty.");
        }

        var prompt = """
            You are an AI workflow assistant.

            Analyze the document below and produce:
            1. A concise summary.
            2. A list of actionable tasks identified from the document.

            Return ONLY valid JSON in exactly this format:

            {
              "summary": "Concise summary of the document.",
              "actionItems": [
                "Action item 1",
                "Action item 2"
              ]
            }

            Rules:
            - Do not invent information.
            - Only include action items supported by the document.
            - If there are no action items, return an empty array.
            - Do not include markdown.
            - Do not include code fences.
            - Do not include any additional properties.

            Document:

            """ + documentContent;

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(
                "You are a reliable AI workflow assistant. " +
                "Always return valid JSON."),

            new UserChatMessage(prompt)
        };

        var response = await _chatClient.CompleteChatAsync(
            messages);

        var content = response.Value.Content[0].Text;

        var result = JsonSerializer.Deserialize<AiSummaryDto>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (result == null)
        {
            throw new InvalidOperationException(
                "Azure OpenAI returned an invalid document summary.");
        }

        result.Status = "Completed";

        return result;
    }

    public async Task<SpreadsheetAnalysisDto> AnalyzeSpreadsheetAsync(
        SpreadsheetDataDto spreadsheet)
    {
        if (spreadsheet == null)
        {
            throw new ArgumentNullException(nameof(spreadsheet));
        }

        var spreadsheetJson = JsonSerializer.Serialize(
            spreadsheet,
            new JsonSerializerOptions
            {
                WriteIndented = false
            });

        var prompt = """
            You are an AI workflow assistant specializing in
            spreadsheet data quality analysis.

            Analyze the spreadsheet below.

            Identify:
            - Missing values
            - Inconsistent values
            - Invalid or suspicious data
            - Other meaningful data quality issues

            For every issue provide:
            - rowNumber
            - columnName
            - issue
            - currentValue
            - suggestedValue

            Return ONLY valid JSON in exactly this format:

            {
              "summary": "Concise analysis of the spreadsheet.",
              "issues": [
                {
                  "rowNumber": 2,
                  "columnName": "Email",
                  "issue": "Missing value",
                  "currentValue": "",
                  "suggestedValue": null
                }
              ],
              "recommendations": [
                "Recommendation 1",
                "Recommendation 2"
              ]
            }

            Rules:
            - Do not invent data.
            - Only report genuine issues found in the spreadsheet.
            - suggestedValue must be null when a safe correction cannot
              be inferred.
            - Do not guess missing information.
            - Return an empty issues array if no issues exist.
            - Return an empty recommendations array if none are needed.
            - Do not include markdown.
            - Do not include code fences.
            - Do not include additional properties.

            Spreadsheet:

            """ + spreadsheetJson;

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(
                "You are a reliable spreadsheet data analysis assistant. " +
                "Always return valid JSON matching the requested structure."),

            new UserChatMessage(prompt)
        };

        var response = await _chatClient.CompleteChatAsync(
            messages);

        var content = response.Value.Content[0].Text;

        var result = JsonSerializer.Deserialize<SpreadsheetAnalysisDto>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (result == null)
        {
            throw new InvalidOperationException(
                "Azure OpenAI returned an invalid spreadsheet analysis.");
        }

        result.Status = "Completed";

        return result;
    }
}
