using Microsoft.EntityFrameworkCore.Migrations;

namespace EFCorePractice.Migrations
{
    public partial class AddTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorBiography_Authors_AuthorId",
                table: "AuthorBiography");

            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Address_AddressId",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publisher_PublisherId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Publisher_Address_AddressId",
                table: "Publisher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Publisher",
                table: "Publisher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorBiography",
                table: "AuthorBiography");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                table: "Address");

            migrationBuilder.RenameTable(
                name: "Publisher",
                newName: "Publishers");

            migrationBuilder.RenameTable(
                name: "AuthorBiography",
                newName: "AuthorBiographies");

            migrationBuilder.RenameTable(
                name: "Address",
                newName: "Addresses");

            migrationBuilder.RenameIndex(
                name: "IX_Publisher_AddressId",
                table: "Publishers",
                newName: "IX_Publishers_AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorBiography_AuthorId",
                table: "AuthorBiographies",
                newName: "IX_AuthorBiographies_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Publishers",
                table: "Publishers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorBiographies",
                table: "AuthorBiographies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorBiographies_Authors_AuthorId",
                table: "AuthorBiographies",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Addresses_AddressId",
                table: "Authors",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Publishers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Publishers_Addresses_AddressId",
                table: "Publishers",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorBiographies_Authors_AuthorId",
                table: "AuthorBiographies");

            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Addresses_AddressId",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Publishers_Addresses_AddressId",
                table: "Publishers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Publishers",
                table: "Publishers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorBiographies",
                table: "AuthorBiographies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "Publishers",
                newName: "Publisher");

            migrationBuilder.RenameTable(
                name: "AuthorBiographies",
                newName: "AuthorBiography");

            migrationBuilder.RenameTable(
                name: "Addresses",
                newName: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_Publishers_AddressId",
                table: "Publisher",
                newName: "IX_Publisher_AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorBiographies_AuthorId",
                table: "AuthorBiography",
                newName: "IX_AuthorBiography_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Publisher",
                table: "Publisher",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorBiography",
                table: "AuthorBiography",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                table: "Address",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorBiography_Authors_AuthorId",
                table: "AuthorBiography",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Publisher_Address_AddressId",
                table: "Publisher",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
