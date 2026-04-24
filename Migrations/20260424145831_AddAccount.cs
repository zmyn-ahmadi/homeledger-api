using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HomeLedger.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkDaysId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    WeekStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DaysWorked = table.Column<bool[]>(type: "boolean[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkDays", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_WorkDaysId",
                table: "Users",
                column: "WorkDaysId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_WorkDays_WorkDaysId",
                table: "Users",
                column: "WorkDaysId",
                principalTable: "WorkDays",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_WorkDays_WorkDaysId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "WorkDays");

            migrationBuilder.DropIndex(
                name: "IX_Users_WorkDaysId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WorkDaysId",
                table: "Users");
        }
    }
}
