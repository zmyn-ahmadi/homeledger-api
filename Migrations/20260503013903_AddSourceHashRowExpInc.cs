using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeLedger.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceHashRowExpInc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceHash",
                table: "Incomes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceHash",
                table: "Expenses",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceHash",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "SourceHash",
                table: "Expenses");
        }
    }
}
