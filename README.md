# AI Workflow Assistant

An AI-powered workflow automation API built with **C# and ASP.NET Core 8** for processing documents and spreadsheets, extracting useful information, identifying data-quality issues, generating AI-driven insights, and producing structured output files.

The project is designed around a practical workflow:

> **Input file → Extraction/Analysis → AI Processing → Generated Output → Database Persistence**

It supports two primary processing workflows:

* **Document Processing** — PDF and DOCX files
* **Spreadsheet Processing** — CSV and XLSX files

The application exposes REST APIs that can be used directly through Swagger or integrated into automated workflows such as **Microsoft Power Automate and OneDrive**.

---

## Features

### Document Processing

The document workflow supports:

* PDF document uploads
* DOCX document uploads
* Text extraction from uploaded documents
* AI-generated document summaries
* AI-generated action items
* Processing status tracking
* Generated PDF summaries
* Generated DOCX summaries
* Persistence of processed document information in SQL Server

### Spreadsheet Processing

The spreadsheet workflow supports:

* CSV file processing
* XLSX file processing
* Spreadsheet data extraction
* Dynamic column/data handling
* Detection of missing fields
* Detection of inconsistent data
* AI-assisted spreadsheet analysis
* Data-quality reporting
* Generation of structured XLSX output reports

The spreadsheet workflow is intended to do more than simply convert a CSV into an Excel file. It analyses spreadsheet data and identifies issues that can require attention, such as missing or inconsistent values, before producing a structured Excel report.

### AI Processing

The application supports AI-powered processing for extracting useful information from uploaded data.

The AI layer is abstracted behind an interface so that the application is not tightly coupled to a specific implementation.

The project supports:

* AI-generated summaries
* AI-generated action items
* AI-assisted spreadsheet analysis
* AI-driven data-quality handling
* Configurable AI service implementations
* Azure OpenAI integration

A fake AI service is also available for development and testing without requiring an external AI request.

### Persistence

Processed information is persisted using:

* Entity Framework Core
* SQL Server

The application stores information such as:

* Original filename
* File type
* Extracted content
* AI-generated summary
* Action items
* Processing status
* Generated output path
* Processing timestamp

Spreadsheet processing also persists the relevant processing information through the application's database layer.

### Workflow Automation

The API can be integrated into automated workflows.

One of the intended integrations is:

```text
OneDrive
   │
   ▼
Power Automate
   │
   ▼
AI Workflow Assistant API
   │
   ├── Document Processing
   │
   └── Spreadsheet Processing
          │
          ▼
     AI Analysis
          │
          ▼
     Generated Output
```

Power Automate can send file bytes to the API using an `application/octet-stream` request together with the original filename.

This allows the application to become part of a larger automated document and data-processing workflow rather than functioning only as a standalone API.

---

# Architecture

The application follows a layered service-based architecture.

```text
                         ┌──────────────────────┐
                         │   Power Automate     │
                         │      / OneDrive      │
                         └──────────┬───────────┘
                                    │
                                    ▼
                         ┌──────────────────────┐
                         │    ASP.NET Core API  │
                         │      Controllers     │
                         └──────────┬───────────┘
                                    │
                                    ▼
                         ┌──────────────────────┐
                         │    Service Layer     │
                         └──────────┬───────────┘
                                    │
                 ┌──────────────────┴──────────────────┐
                 │                                     │
                 ▼                                     ▼
       ┌──────────────────┐                  ┌──────────────────┐
       │ Document Workflow│                  │Spreadsheet       │
       │                  │                  │Workflow          │
       └────────┬─────────┘                  └────────┬─────────┘
                │                                     │
                ▼                                     ▼
       PDF / DOCX Extraction                   CSV / XLSX Analysis
                │                                     │
                └──────────────────┬──────────────────┘
                                   │
                                   ▼
                         ┌──────────────────────┐
                         │      AI Service      │
                         │   Azure OpenAI /     │
                         │      Fake AI         │
                         └──────────┬───────────┘
                                    │
                         ┌──────────┴───────────┐
                         ▼                      ▼
                Generated Documents       Generated XLSX
                         │                      │
                         └──────────┬───────────┘
                                    ▼
                         ┌──────────────────────┐
                         │    Entity Framework  │
                         │       Core           │
                         └──────────┬───────────┘
                                    ▼
                         ┌──────────────────────┐
                         │      SQL Server      │
                         └──────────────────────┘
```

---

# Document Workflow

The document pipeline is responsible for processing unstructured documents and turning them into structured AI-generated results.

## Supported Formats

```text
.pdf
.docx
```

## Processing Flow

```text
PDF / DOCX
    │
    ▼
File Upload
    │
    ▼
File Type Detection
    │
    ▼
Document Extractor
    │
    ├── PDF Extractor
    │
    └── DOCX Extractor
    │
    ▼
Extracted Text
    │
    ▼
AI Service
    │
    ├── Summary
    └── Action Items
    │
    ▼
Generated Summary File
    │
    ├── PDF
    └── DOCX
    │
    ▼
Database Persistence
```

The extraction layer uses an `IDocumentExtractor` abstraction so different document formats can have their own implementations.

For example:

```csharp
public interface IDocumentExtractor
{
    bool CanHandle(string fileExtension);

    Task<string> ExtractTextAsync(Stream fileStream);
}
```

This allows the application to select the appropriate extractor dynamically based on the uploaded file type.

---

# Spreadsheet Workflow

The spreadsheet pipeline is a separate processing workflow designed around **structured data analysis and data quality**.

Supported input formats:

```text
.csv
.xlsx
```

The purpose of this workflow is not simply to export spreadsheet data into another file.

Instead, the application can analyse spreadsheet data and identify issues such as:

* Missing values
* Missing fields
* Inconsistent values
* Data-quality problems
* Irregularities in spreadsheet records

The resulting information can then be used to generate a structured Excel report.

## Spreadsheet Processing Flow

```text
CSV / XLSX
    │
    ▼
Spreadsheet Upload
    │
    ▼
Spreadsheet Extraction
    │
    ▼
Data Analysis
    │
    ├── Missing Fields
    ├── Inconsistent Data
    └── Data Quality Issues
    │
    ▼
AI-Assisted Analysis
    │
    ▼
Structured XLSX Report
    │
    ▼
Database Persistence
```

Both CSV and XLSX inputs can ultimately produce an XLSX report.

```text
CSV
 │
 ▼
Analysis
 │
 ▼
XLSX Report
```

and:

```text
XLSX
 │
 ▼
Analysis
 │
 ▼
XLSX Report
```

This makes the spreadsheet workflow useful for scenarios where incoming spreadsheet data needs to be checked and reported on before being used downstream.

---

# AI Service

AI functionality is isolated behind an application-level interface rather than being embedded directly inside controllers or document-processing logic.

This provides separation between:

* File processing
* Business logic
* AI processing
* Persistence

The application can therefore switch between implementations such as:

```text
IAIService
   │
   ├── FakeAIService
   │
   └── Azure OpenAI implementation
```

The fake implementation is useful during development because the application can be tested without making external AI calls.

The Azure OpenAI implementation provides the production AI workflow.

---

# API Endpoints

The application exposes separate endpoints for document and spreadsheet processing.

## Document Processing

### Process a Document

```http
POST /api/Document/process
```

This endpoint processes a PDF or DOCX document and returns information about the processed document.

### Process a Raw File

```http
POST /api/Document/process-file
```

This endpoint accepts the file as raw request data.

The filename is supplied using:

```http
X-File-Name: meeting-notes.docx
```

with:

```http
Content-Type: application/octet-stream
```

The API uses the filename extension to determine which processing workflow should be used.

### Retrieve Generated Document

```http
GET /api/Document/{id}/file
```

This endpoint retrieves the generated PDF or DOCX associated with a processed document.

---

# Spreadsheet API

### Analyse a Spreadsheet

```http
POST /api/Spreadsheet/analyze
```

This endpoint accepts CSV or XLSX spreadsheet data, analyses the data, and returns the generated XLSX report.

---

# Example Document Request

A raw document request can look like:

```http
POST /api/Document/process-file
Content-Type: application/octet-stream
X-File-Name: test-meeting.pdf
```

The request body contains the document bytes.

The same mechanism can be used for DOCX files:

```http
POST /api/Document/process-file
Content-Type: application/octet-stream
X-File-Name: test-meeting.docx
```

The filename is important because it is used to determine the document type.

---

# Generated Outputs

The application generates output files based on the type of workflow being executed.

## Documents

For an input such as:

```text
meeting-notes.docx
```

the generated file follows the pattern:

```text
meeting-notes-ai-summary.docx
```

For a PDF:

```text
meeting-notes.pdf
```

the generated output follows:

```text
meeting-notes-ai-summary.pdf
```

## Spreadsheets

Spreadsheet processing produces an XLSX report containing the results of the spreadsheet analysis and data-quality processing.

---

# Database

The application uses **Entity Framework Core** with **SQL Server** for persistence.

The database layer is responsible for maintaining a record of processed data rather than relying exclusively on generated files.

The document workflow stores information including:

```text
Document ID
Original File Name
File Type
Original Content
AI Summary
Action Items
Output File Path
Processing Status
Processed At
```

This provides a persistent processing history that can later be exposed through additional API functionality.

---

# Project Structure

```text
AI-Workflow-Assistant/
│
├── src/
│   └── AIWorkflowAssistant.Api/
│       │
│       ├── Controllers/
│       │   ├── DocumentController.cs
│       │   └── SpreadsheetController.cs
│       │
│       ├── Data/
│       │   └── ApplicationDbContext.cs
│       │
│       ├── DTOs/
│       │   ├── DocumentRequestDto.cs
│       │   ├── DocumentResponseDto.cs
│       │   ├── AiSummaryDto.cs
│       │   └── Spreadsheet DTOs
│       │
│       ├── Interfaces/
│       │   ├── IDocumentService.cs
│       │   ├── IDocumentExtractor.cs
│       │   ├── IDocumentFileGenerator.cs
│       │   ├── IAIService.cs
│       │   └── Spreadsheet interfaces
│       │
│       ├── Models/
│       │   └── ProcessedDocument.cs
│       │
│       └── Services/
│           │
│           ├── DocumentService.cs
│           ├── AIService.cs
│           ├── SpreadsheetService.cs
│           │
│           ├── DocumentExtraction/
│           │   ├── PdfDocumentExtractor.cs
│           │   └── DocxDocumentExtractor.cs
│           │
│           └── FileGeneration/
│               └── DocumentFileGenerator.cs
│
└── README.md
```

---

# Technology Stack

## Backend

* **C#**
* **.NET 8**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **SQL Server**

## AI

* **Azure OpenAI**
* AI service abstraction
* Fake AI service for testing

## Documents

* **Open XML SDK** for DOCX processing
* **QuestPDF** for PDF generation

## Spreadsheets

* CSV processing
* XLSX processing
* Spreadsheet data analysis
* XLSX report generation

## API & Development

* Swagger / OpenAPI
* Dependency Injection
* DTO-based API contracts
* Asynchronous programming
* REST APIs

## Cloud & Infrastructure

* Microsoft Azure
* Azure App Service
* Azure OpenAI
* Docker
* SQL Server

## Automation

* Microsoft Power Automate
* OneDrive

---

# Local Development

## Prerequisites

Install:

* .NET 8 SDK
* SQL Server
* Git
* Docker (optional, depending on the local infrastructure configuration)

Clone the repository:

```bash
git clone <repository-url>
cd AI-Workflow-Assistant
```

Navigate to the API:

```bash
cd src/AIWorkflowAssistant.Api
```

Restore dependencies:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

Run the application:

```bash
dotnet run
```

The application exposes Swagger/OpenAPI documentation when running in the development environment.

---

# Database Configuration

The application uses Entity Framework Core for database access.

Configure the SQL Server connection string through application configuration or environment variables.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-connection-string"
  }
}
```

Apply migrations:

```bash
dotnet ef database update
```

Production credentials and secrets should not be committed to source control.

---

# Azure Deployment

The application is deployed to **Azure App Service**.

The deployed environment uses:

```text
ASP.NET Core 8
Azure App Service
Azure OpenAI
SQL Server
Docker
```

The application has been deployed to the **South Africa North** Azure region.

The production API can be tested through its Swagger/OpenAPI interface and through direct HTTP requests.

---

# Power Automate Integration

The API is designed to work with automated workflows.

A Power Automate flow can retrieve a file from OneDrive and send the file content to the API.

Conceptually:

```text
OneDrive
   │
   │ File created/updated
   ▼
Power Automate
   │
   │ Get file content
   ▼
HTTP Request
   │
   │ application/octet-stream
   │ X-File-Name
   ▼
AI Workflow Assistant
   │
   ▼
Process file
   │
   ├── Document workflow
   │
   └── Spreadsheet workflow
   │
   ▼
AI Analysis
   │
   ▼
Generated Output
```

This allows document and spreadsheet processing to happen automatically when files enter a connected workflow.

---

# Important Implementation Detail: Request Streams

The API processes uploaded files as streams.

Because ASP.NET Core/Kestrel can disallow synchronous stream operations, document extraction uses asynchronous stream operations when copying request data into memory.

For example:

```csharp
using var memoryStream = new MemoryStream();

await fileStream.CopyToAsync(memoryStream);

memoryStream.Position = 0;
```

This is important when processing files received directly from HTTP requests, particularly when the application is running in Azure App Service.

The same principle applies to the different file-processing components: the application must correctly handle the incoming request stream before passing it to libraries such as the Open XML SDK.

---

# Error Handling and Validation

The API validates incoming files before processing them.

Validation includes:

* Missing file
* Empty file
* Missing filename
* Unsupported file extension
* Missing extractor
* Invalid document content
* Invalid spreadsheet input

The document workflow currently accepts:

```text
.pdf
.docx
```

The spreadsheet workflow accepts:

```text
.csv
.xlsx
```

The application also uses logging to diagnose runtime issues in local and Azure environments.

---

# Development and Design Principles

The project was built around several backend engineering principles.

### Separation of Concerns

Controllers handle HTTP requests while services handle business logic.

```text
Controller
    ↓
Service
    ↓
Processing / AI / Persistence
```

### Interface-Based Design

Core functionality is abstracted behind interfaces such as:

```text
IDocumentService
IDocumentExtractor
IDocumentFileGenerator
IAIService
```

This makes individual components easier to replace and test.

### Dependency Injection

Services and extractors are registered with ASP.NET Core dependency injection rather than being manually instantiated throughout the application.

### Asynchronous Processing

File and database operations use asynchronous APIs where appropriate.

### Extensibility

The extractor architecture allows additional file formats to be introduced without rewriting the central document-processing workflow.

---

# Current Processing Capabilities

| Input | Processing                | AI                     | Output |
| ----- | ------------------------- | ---------------------- | ------ |
| PDF   | Text extraction           | Summary + action items | PDF    |
| DOCX  | Text extraction           | Summary + action items | DOCX   |
| CSV   | Spreadsheet/data analysis | Data-quality analysis  | XLSX   |
| XLSX  | Spreadsheet/data analysis | Data-quality analysis  | XLSX   |

This gives the project two complementary capabilities:

**Unstructured data processing**

```text
PDF / DOCX
    ↓
Text
    ↓
AI
    ↓
Summary + Action Items
```

**Structured data processing**

```text
CSV / XLSX
    ↓
Spreadsheet Data
    ↓
Data Quality Analysis
    ↓
AI-Assisted Processing
    ↓
XLSX Report
```

---

# Future Improvements

Potential future development includes:

* Authentication and authorization
* More document formats
* Additional spreadsheet formats
* More advanced spreadsheet data validation
* Document download and management endpoints
* Processing-history endpoints
* Azure Blob Storage for generated files
* Background processing for large files
* More automated integration tests
* Improved structured logging
* Centralized exception handling
* More sophisticated AI prompts
* Additional AI providers
* A dedicated frontend for file uploads and results
* Expanded Power Automate workflows

---

# Project Goals

The goal of AI Workflow Assistant is to demonstrate how an AI service can be integrated into a practical backend automation system rather than being used as an isolated chatbot.

The project combines:

* File ingestion
* Document extraction
* Spreadsheet analysis
* Data-quality processing
* AI integration
* File generation
* Database persistence
* REST APIs
* Workflow automation
* Cloud deployment

The result is a backend system capable of taking real-world business files, processing their contents, using AI to generate useful results, and returning structured outputs that can be consumed by people or automated workflows.

---

# License

This project is a personal portfolio and learning project.
