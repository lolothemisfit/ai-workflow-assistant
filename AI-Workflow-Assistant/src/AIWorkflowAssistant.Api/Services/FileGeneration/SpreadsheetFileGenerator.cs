using AIWorkflowAssistant.Api.DTOs;
using AIWorkflowAssistant.Api.Interfaces;
using ClosedXML.Excel;

namespace AIWorkflowAssistant.Api.Services.FileGeneration;

public class SpreadsheetFileGenerator : ISpreadsheetFileGenerator
{
    private readonly IWebHostEnvironment _environment;

    public SpreadsheetFileGenerator(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public Task<string> GenerateAsync(
        string originalFileName,
        SpreadsheetDataDto spreadsheet,
        SpreadsheetAnalysisDto analysis)
    {
        var outputDirectory = Path.Combine(
            _environment.ContentRootPath,
            "GeneratedFiles");

        Directory.CreateDirectory(outputDirectory);

        var originalName = Path.GetFileNameWithoutExtension(
            originalFileName);

        var outputFileName =
            $"{originalName}_processed_{DateTime.UtcNow:yyyy-MM-dd-tt}.xlsx";

        var outputPath = Path.Combine(
            outputDirectory,
            outputFileName);

        using var workbook = new XLWorkbook();

        GenerateDataWorksheet(
            workbook,
            spreadsheet);

        GenerateAnalysisWorksheet(
            workbook,
            analysis);

        workbook.SaveAs(outputPath);

        return Task.FromResult(outputPath);
    }

    // ============================================================
    // DATA WORKSHEET
    // ============================================================

    private static void GenerateDataWorksheet(
        XLWorkbook workbook,
        SpreadsheetDataDto spreadsheet)
    {
        var worksheet = workbook.Worksheets.Add("Data");

        if (spreadsheet.Headers.Count == 0)
        {
            worksheet.Cell("A1").Value = "No spreadsheet headers found.";
            worksheet.Cell("A1").Style.Font.Bold = true;
            return;
        }

        // --------------------------------------------------------
        // Title
        // --------------------------------------------------------

        worksheet.Cell("A1").Value = "Processed Spreadsheet";

        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;

        var lastColumn = spreadsheet.Headers.Count;

        worksheet.Range(
            1,
            1,
            1,
            lastColumn)
            .Merge();

        // --------------------------------------------------------
        // Headers
        // --------------------------------------------------------

        const int headerRow = 3;

        for (var column = 0;
             column < spreadsheet.Headers.Count;
             column++)
        {
            worksheet.Cell(
                headerRow,
                column + 1)
                .Value = spreadsheet.Headers[column];
        }

        var lastDataRow =
            headerRow + spreadsheet.Rows.Count;

        // --------------------------------------------------------
        // Data
        // --------------------------------------------------------

        for (var row = 0;
             row < spreadsheet.Rows.Count;
             row++)
        {
            var rowData = spreadsheet.Rows[row];

            for (var column = 0;
                 column < spreadsheet.Headers.Count;
                 column++)
            {
                var header = spreadsheet.Headers[column];

                rowData.TryGetValue(
                    header,
                    out var value);

                worksheet.Cell(
                    row + headerRow + 1,
                    column + 1)
                    .Value = value ?? string.Empty;
            }
        }

        // --------------------------------------------------------
        // Header styling
        // --------------------------------------------------------

        var headerRange = worksheet.Range(
            headerRow,
            1,
            headerRow,
            lastColumn);

        headerRange.Style.Font.Bold = true;
        headerRange.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        headerRange.Style.Border.BottomBorder =
            XLBorderStyleValues.Thin;

        // --------------------------------------------------------
        // Create Excel table
        // --------------------------------------------------------

        if (spreadsheet.Rows.Count > 0)
        {
            var dataRange = worksheet.Range(
                headerRow,
                1,
                lastDataRow,
                lastColumn);

            var table = dataRange.CreateTable();

            table.Name = "ProcessedData";

            table.Theme = XLTableTheme.TableStyleMedium2;
        }

        // --------------------------------------------------------
        // Freeze headers
        // --------------------------------------------------------

        worksheet.SheetView.FreezeRows(headerRow);

        // --------------------------------------------------------
        // Formatting
        // --------------------------------------------------------

        worksheet.Columns().AdjustToContents();

        // Prevent extremely narrow columns.
        for (var column = 1;
             column <= lastColumn;
             column++)
        {
            if (worksheet.Column(column).Width < 12)
            {
                worksheet.Column(column).Width = 12;
            }
        }

        // Prevent excessively wide columns.
        for (var column = 1;
             column <= lastColumn;
             column++)
        {
            if (worksheet.Column(column).Width > 40)
            {
                worksheet.Column(column).Width = 40;
            }
        }
    }

    // ============================================================
    // AI ANALYSIS WORKSHEET
    // ============================================================

    private static void GenerateAnalysisWorksheet(
        XLWorkbook workbook,
        SpreadsheetAnalysisDto analysis)
    {
        var worksheet = workbook.Worksheets.Add("AI Analysis");

        // --------------------------------------------------------
        // Title
        // --------------------------------------------------------

        worksheet.Cell("A1").Value =
            "AI Spreadsheet Analysis";

        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 18;

        worksheet.Range("A1:E1").Merge();

        // --------------------------------------------------------
        // Summary
        // --------------------------------------------------------

        worksheet.Cell("A3").Value = "Summary";
        worksheet.Cell("A3").Style.Font.Bold = true;
        worksheet.Cell("A3").Style.Font.FontSize = 13;

        worksheet.Range("A3:E3").Merge();

        worksheet.Cell("A4").Value =
            analysis.Summary;

        worksheet.Range("A4:E5").Merge();

        worksheet.Cell("A4").Style.Alignment.WrapText = true;
        worksheet.Cell("A4").Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Top;

        // --------------------------------------------------------
        // Status
        // --------------------------------------------------------

        worksheet.Cell("A7").Value = "Status";
        worksheet.Cell("A7").Style.Font.Bold = true;

        worksheet.Cell("B7").Value =
            analysis.Status;

        worksheet.Cell("B7").Style.Font.Bold = true;

        // --------------------------------------------------------
        // Issues
        // --------------------------------------------------------

        worksheet.Cell("A9").Value = "Issues";
        worksheet.Cell("A9").Style.Font.Bold = true;
        worksheet.Cell("A9").Style.Font.FontSize = 13;

        worksheet.Range("A9:E9").Merge();

        const int issueHeaderRow = 10;

        worksheet.Cell(issueHeaderRow, 1).Value = "Row";
        worksheet.Cell(issueHeaderRow, 2).Value = "Column";
        worksheet.Cell(issueHeaderRow, 3).Value = "Issue";
        worksheet.Cell(issueHeaderRow, 4).Value = "Current Value";
        worksheet.Cell(issueHeaderRow, 5).Value = "Suggested Value";

        var issueHeaderRange = worksheet.Range(
            issueHeaderRow,
            1,
            issueHeaderRow,
            5);

        issueHeaderRange.Style.Font.Bold = true;
        issueHeaderRange.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;
        issueHeaderRange.Style.Border.BottomBorder =
            XLBorderStyleValues.Thin;

        var issueRow = issueHeaderRow + 1;

        foreach (var issue in analysis.Issues)
        {
            worksheet.Cell(issueRow, 1)
                .Value = issue.RowNumber;

            worksheet.Cell(issueRow, 2)
                .Value = issue.ColumnName;

            worksheet.Cell(issueRow, 3)
                .Value = issue.Issue;

            worksheet.Cell(issueRow, 4)
                .Value = issue.CurrentValue ?? string.Empty;

            worksheet.Cell(issueRow, 5)
                .Value = issue.SuggestedValue ?? string.Empty;

            worksheet.Range(
                issueRow,
                1,
                issueRow,
                5)
                .Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            issueRow++;
        }

        // --------------------------------------------------------
        // Recommendations
        // --------------------------------------------------------

        var recommendationStartRow =
            issueRow + 2;

        worksheet.Cell(
            recommendationStartRow,
            1)
            .Value = "Recommendations";

        worksheet.Cell(
            recommendationStartRow,
            1)
            .Style.Font.Bold = true;

        worksheet.Cell(
            recommendationStartRow,
            1)
            .Style.Font.FontSize = 13;

        worksheet.Range(
            recommendationStartRow,
            1,
            recommendationStartRow,
            5)
            .Merge();

        var recommendationRow =
            recommendationStartRow + 1;

        foreach (var recommendation
                 in analysis.Recommendations)
        {
            worksheet.Cell(
                recommendationRow,
                1)
                .Value = $"• {recommendation}";

            worksheet.Range(
                recommendationRow,
                1,
                recommendationRow,
                5)
                .Merge();

            worksheet.Cell(
                recommendationRow,
                1)
                .Style.Alignment.WrapText = true;

            recommendationRow++;
        }

        // --------------------------------------------------------
        // General formatting
        // --------------------------------------------------------

        worksheet.Column(1).Width = 12;
        worksheet.Column(2).Width = 20;
        worksheet.Column(3).Width = 30;
        worksheet.Column(4).Width = 30;
        worksheet.Column(5).Width = 30;

        worksheet.Row(4).Height = 30;
        worksheet.Row(5).Height = 30;

        worksheet.SheetView.FreezeRows(10);
    }
}