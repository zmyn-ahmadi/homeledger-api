using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeLedger.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceHashRows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "uq_income",
                table: "Incomes",
                column: "SourceHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_expense",
                table: "Expenses",
                column: "SourceHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_income",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "uq_expense",
                table: "Expenses");
        }
    }
}
