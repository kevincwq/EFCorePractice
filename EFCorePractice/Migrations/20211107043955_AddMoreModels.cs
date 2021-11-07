using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EFCorePractice.Migrations
{
    public partial class AddMoreModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PublisherId",
                table: "Books",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                table: "Authors",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Street = table.Column<string>(type: "TEXT", nullable: true),
                    StateOrProvince = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthorBiography",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Biography = table.Column<string>(type: "TEXT", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PlaceOfBirth = table.Column<string>(type: "TEXT", nullable: true),
                    Nationality = table.Column<string>(type: "TEXT", nullable: true),
                    AuthorId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorBiography", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorBiography_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Publisher",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    AddressId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publisher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publisher_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_PublisherId",
                table: "Books",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_AddressId",
                table: "Authors",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorBiography_AuthorId",
                table: "AuthorBiography",
                column: "AuthorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Publisher_AddressId",
                table: "Publisher",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Address_AddressId",
                table: "Authors",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publisher_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Publisher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Address_AddressId",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publisher_PublisherId",
                table: "Books");

            migrationBuilder.DropTable(
                name: "AuthorBiography");

            migrationBuilder.DropTable(
                name: "Publisher");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Books_PublisherId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Authors_AddressId",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "PublisherId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Authors");
        }
    }
}
