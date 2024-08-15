using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MorixChatService.Migrations
{
    /// <inheritdoc />
    public partial class FixedUserType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserType",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserType",
                table: "AspNetUsers",
                column: "UserType",
                unique: true,
                filter: "[UserType] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserType",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserType",
                table: "AspNetUsers",
                column: "UserType",
                unique: true,
                filter: "[UserType] = 0");
        }
    }
}
