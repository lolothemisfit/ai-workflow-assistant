using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIWorkflowAssistant.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddActionItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionItems",
                table: "ProcessedDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionItems",
                table: "ProcessedDocuments");
        }
    }
}
