using AIWorkflowAssistant.Api.Interfaces;
using AIWorkflowAssistant.Api.Services;
using AIWorkflowAssistant.Api.Data;
using Microsoft.EntityFrameworkCore;
using AIWorkflowAssistant.Api.Services.DocumentExtraction;
using AIWorkflowAssistant.Api.Services.FileGeneration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IAIService, AIService>();
// builder.Services.AddScoped<IAIService, FakeAIService>();
builder.Services.AddScoped<IDocumentExtractor, DocxDocumentExtractor>();
builder.Services.AddScoped<IDocumentExtractor, PdfDocumentExtractor>();
builder.Services.AddScoped<ISpreadsheetService, SpreadsheetService>();
builder.Services.AddScoped<IDocumentFileGenerator, DocumentFileGenerator>();
builder.Services.AddScoped<ISpreadsheetFileGenerator, SpreadsheetFileGenerator>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

