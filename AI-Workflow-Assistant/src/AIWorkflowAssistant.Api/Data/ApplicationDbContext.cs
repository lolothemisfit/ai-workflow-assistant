using AIWorkflowAssistant.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AIWorkflowAssistant.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProcessedDocument> ProcessedDocuments { get; set; }
}