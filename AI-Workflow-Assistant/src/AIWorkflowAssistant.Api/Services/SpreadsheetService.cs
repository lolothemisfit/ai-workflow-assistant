using System.Text.Json;
using AIWorkflowAssistant.Api.Data;
using AIWorkflowAssistant.Api.DTOs;
using AIWorkflowAssistant.Api.Interfaces;
using AIWorkflowAssistant.Api.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;

namespace AIWorkflowAssistant.Api.Services;

public class SpreadsheetService : ISpreadsheetService
{
    private readonly ApplicationDbContext _context;
    private readonly IAIService _aiService;

    public SpreadsheetService(
        ApplicationDbContext context,
        IAIService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<SpreadsheetDataDto> ExtractAsync(
        IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException(
                "A spreadsheet file is required.");
        }

        var extension = Path.GetExtension(file.FileName)
            .ToLowerInvariant();

        return extension switch
        {
            ".csv" => await ExtractCsvAsync(file),
            ".xlsx" => await ExtractExcelAsync(file),
            _ => throw new ArgumentException(
                "Only CSV and XLSX files are supported.")
        };
    }

    public async Task<SpreadsheetAnalysisDto> ProcessAsync(
        IFormFile file)
    {
        var spreadsheet = await ExtractAsync(file);

        var aiResult =
            await _aiService.AnalyzeSpreadsheetAsync(
                spreadsheet);

        var extension = Path.GetExtension(file.FileName)
            .ToLowerInvariant();

        var originalContent =
            JsonSerializer.Serialize(spreadsheet);

        var document = new ProcessedDocument
        {
            OriginalFileName = file.FileName,
            FileType = extension,
            OriginalContent = originalContent,
            AiSummary = aiResult.Summary,
            ActionItems = JsonSerializer.Serialize(
                aiResult.Recommendations),
            Status = aiResult.Status,
            ProcessedAt = DateTime.UtcNow
        };

        _context.ProcessedDocuments.Add(document);

        await _context.SaveChangesAsync();

        return aiResult;
    }

    private async Task<SpreadsheetDataDto> ExtractCsvAsync(
        IFormFile file)
    {
        var result = new SpreadsheetDataDto();

        using var stream = file.OpenReadStream();
        using var reader = new TextFieldParser(stream);

        reader.TextFieldType = FieldType.Delimited;
        reader.SetDelimiters(",");
        reader.HasFieldsEnclosedInQuotes = true;

        if (reader.EndOfData)
        {
            throw new ArgumentException(
                "The CSV file is empty.");
        }

        var headers = reader.ReadFields();

        if (headers == null || headers.Length == 0)
        {
            throw new ArgumentException(
                "The CSV file does not contain headers.");
        }

        result.Headers = headers
            .Select(header => header.Trim())
            .ToList();

        while (!reader.EndOfData)
        {
            var fields = reader.ReadFields();

            if (fields == null)
            {
                continue;
            }

            var row = new Dictionary<string, string>();

            for (var i = 0; i < result.Headers.Count; i++)
            {
                var value = i < fields.Length
                    ? fields[i].Trim()
                    : string.Empty;

                row[result.Headers[i]] = value;
            }

            result.Rows.Add(row);
        }

        return result;
    }

    private async Task<SpreadsheetDataDto> ExtractExcelAsync(
        IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheets.First();

        var result = new SpreadsheetDataDto();

        var firstRow = worksheet.FirstRowUsed();

        if (firstRow == null)
        {
            return result;
        }

        var headerCells = firstRow.CellsUsed();

        foreach (var cell in headerCells)
        {
            result.Headers.Add(
                cell.GetString().Trim());
        }

        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            var rowData =
                new Dictionary<string, string>();

            for (var i = 0;
                 i < result.Headers.Count;
                 i++)
            {
                var cell = row.Cell(i + 1);

                rowData[result.Headers[i]] =
                    cell.GetString().Trim();
            }

            result.Rows.Add(rowData);
        }

        return result;
    }
}
