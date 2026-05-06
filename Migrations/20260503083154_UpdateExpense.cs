using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeLedger.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_expense",
                table: "Expenses");

            migrationBuilder.AlterColumn<string>(
                name: "SourceHash",
                table: "Expenses",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "uq_expense",
                table: "Expenses",
                column: "SourceHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "uq_expense",
                table: "Expenses");

            migrationBuilder.AlterColumn<string>(
                name: "SourceHash",
                table: "Expenses",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "uq_expense",
                table: "Expenses",
                column: "SourceHash",
                unique: true);
        }
    }
}
