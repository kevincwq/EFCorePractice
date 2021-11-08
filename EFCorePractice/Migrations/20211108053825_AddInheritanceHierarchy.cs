using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EFCorePractice.Migrations
{
    public partial class AddInheritanceHierarchy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Months = table.Column<int>(type: "INTEGER", nullable: false),
                    Charge = table.Column<decimal>(type: "TEXT", nullable: false),
                    ContractType = table.Column<int>(type: "INTEGER", nullable: false),
                    DownloadSpeed = table.Column<int>(type: "INTEGER", nullable: true),
                    MobileNumber = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");
        }
    }
}
