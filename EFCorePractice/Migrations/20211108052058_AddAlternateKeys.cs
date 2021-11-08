using Microsoft.EntityFrameworkCore.Migrations;

namespace EFCorePractice.Migrations
{
    public partial class AddAlternateKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Isbn",
                table: "Books",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_User_UserName",
                table: "User",
                column: "UserName");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Publishers_Name",
                table: "Publishers",
                column: "Name");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Contacts_Email",
                table: "Contacts",
                column: "Email");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Categories_Name",
                table: "Categories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Isbn",
                table: "Books",
                column: "Isbn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title",
                table: "Books",
                column: "Title");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_User_UserName",
                table: "User");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Publishers_Name",
                table: "Publishers");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Contacts_Email",
                table: "Contacts");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Categories_Name",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Books_Isbn",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_Title",
                table: "Books");

            migrationBuilder.AlterColumn<string>(
                name: "Isbn",
                table: "Books",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
