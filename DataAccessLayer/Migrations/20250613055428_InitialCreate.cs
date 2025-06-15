using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BetData",
                columns: table => new
                {
                    BetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DrawDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DrawTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    BetTotal = table.Column<int>(type: "int", nullable: false),
                    WinAmount = table.Column<int>(type: "int", nullable: true),
                    Winner = table.Column<int>(type: "int", nullable: true),
                    XValue = table.Column<int>(type: "int", nullable: true),
                    Bet0 = table.Column<int>(type: "int", nullable: false),
                    Bet1 = table.Column<int>(type: "int", nullable: false),
                    Bet2 = table.Column<int>(type: "int", nullable: false),
                    Bet3 = table.Column<int>(type: "int", nullable: false),
                    Bet4 = table.Column<int>(type: "int", nullable: false),
                    Bet5 = table.Column<int>(type: "int", nullable: false),
                    Bet6 = table.Column<int>(type: "int", nullable: false),
                    Bet7 = table.Column<int>(type: "int", nullable: false),
                    Bet8 = table.Column<int>(type: "int", nullable: false),
                    Bet9 = table.Column<int>(type: "int", nullable: false),
                    Bet10 = table.Column<int>(type: "int", nullable: false),
                    DeleteStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BetData", x => x.BetId);
                });

            migrationBuilder.CreateTable(
                name: "GameSetting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Percentage = table.Column<int>(type: "int", nullable: false),
                    Buffer = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Balance = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Percentage = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReferName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ReferId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SDId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    UniqueId = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "WinningNumber",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalWin = table.Column<int>(type: "int", nullable: false),
                    Winner = table.Column<int>(type: "int", nullable: false),
                    DrawDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DrawTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalPlayed = table.Column<int>(type: "int", nullable: false),
                    Profit = table.Column<int>(type: "int", nullable: false),
                    WinMod = table.Column<int>(type: "int", nullable: false),
                    XValue = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WinningNumber", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Balance", "DateTime", "DeleteStatus", "Note", "Password", "Percentage", "ReferId", "ReferName", "Role", "Status", "SId", "UniqueId", "Username" },
                values: new object[] { 1, 2147483647, new DateTime(2025, 6, 13, 11, 24, 23, 376, DateTimeKind.Local).AddTicks(942), 1, "Initial SuperAdmin User", "admin123", 0, null, "superadmin", "superadmin", 1, 1, "123456", "superadmin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BetData");

            migrationBuilder.DropTable(
                name: "GameSetting");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WinningNumber");
        }
    }
}
