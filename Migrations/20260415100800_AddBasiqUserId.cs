using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeLedger.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBasiqUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BasiqUserId",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasiqUserId",
                table: "Users");
        }
    }
}
