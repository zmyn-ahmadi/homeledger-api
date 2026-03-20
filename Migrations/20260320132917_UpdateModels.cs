using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HomeLedger.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillShares_Users_UserId",
                table: "BillShares");

            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_Users_UserId",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Users_UserId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Users_UserId",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_User_Date",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "DateJoined",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserProfiles",
                newName: "UserProfileId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Payments",
                newName: "BillPaymentId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Incomes",
                newName: "IncomeId");

            migrationBuilder.RenameIndex(
                name: "IX_Incomes_UserId",
                table: "Incomes",
                newName: "IX_Incomes_UserId1");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Expenses",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Expenses",
                newName: "ExpenseId");

            migrationBuilder.RenameIndex(
                name: "IX_Expenses_UserId",
                table: "Expenses",
                newName: "IX_Expenses_OwnerId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Categories",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Categories",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_UserId",
                table: "Categories",
                newName: "IX_Categories_OwnerId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Budgets",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Budgets",
                newName: "BudgetId");

            migrationBuilder.RenameIndex(
                name: "IX_Budgets_UserId",
                table: "Budgets",
                newName: "IX_Budgets_OwnerId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BillShares",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_BillShares_UserId",
                table: "BillShares",
                newName: "IX_BillShares_OwnerId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Bills",
                newName: "BillId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserProfiles",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Payments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Incomes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Incomes",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Incomes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Incomes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Expenses",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Expenses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Categories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Budgets",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Budgets",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Budgets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Bills",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Bills",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Bills",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_User_Date",
                table: "Incomes",
                columns: new[] { "OwnerId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_UserId",
                table: "Incomes",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillShares_Users_OwnerId",
                table: "BillShares",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_Users_OwnerId",
                table: "Budgets",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_OwnerId",
                table: "Categories",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Users_OwnerId",
                table: "Expenses",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Users_OwnerId",
                table: "Incomes",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Users_UserId",
                table: "Incomes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillShares_Users_OwnerId",
                table: "BillShares");

            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_Users_OwnerId",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_OwnerId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Users_OwnerId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Users_OwnerId",
                table: "Incomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_Users_UserId",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_User_Date",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_UserId",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Bills");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "UserProfiles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BillPaymentId",
                table: "Payments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IncomeId",
                table: "Incomes",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Incomes_UserId1",
                table: "Incomes",
                newName: "IX_Incomes_UserId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Expenses",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ExpenseId",
                table: "Expenses",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Expenses_OwnerId",
                table: "Expenses",
                newName: "IX_Expenses_UserId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Categories",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Categories",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_OwnerId",
                table: "Categories",
                newName: "IX_Categories_UserId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Budgets",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "BudgetId",
                table: "Budgets",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Budgets_OwnerId",
                table: "Budgets",
                newName: "IX_Budgets_UserId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "BillShares",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BillShares_OwnerId",
                table: "BillShares",
                newName: "IX_BillShares_UserId");

            migrationBuilder.RenameColumn(
                name: "BillId",
                table: "Bills",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateJoined",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserProfiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Incomes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Incomes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Expenses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Budgets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Budgets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Bills",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Bills",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_User_Date",
                table: "Incomes",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.AddForeignKey(
                name: "FK_BillShares_Users_UserId",
                table: "BillShares",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_Users_UserId",
                table: "Budgets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Users_UserId",
                table: "Expenses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_Users_UserId",
                table: "Incomes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
