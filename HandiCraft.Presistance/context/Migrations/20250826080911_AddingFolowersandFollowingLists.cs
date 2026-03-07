using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandiCraft.Presistance.context.Migrations
{
    /// <inheritdoc />
    public partial class AddingFolowersandFollowingLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "UserFollows",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "UserFollows",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_ApplicationUserId",
                table: "UserFollows",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_ApplicationUserId1",
                table: "UserFollows",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollows_AspNetUsers_ApplicationUserId",
                table: "UserFollows",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollows_AspNetUsers_ApplicationUserId1",
                table: "UserFollows",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollows_AspNetUsers_ApplicationUserId",
                table: "UserFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollows_AspNetUsers_ApplicationUserId1",
                table: "UserFollows");

            migrationBuilder.DropIndex(
                name: "IX_UserFollows_ApplicationUserId",
                table: "UserFollows");

            migrationBuilder.DropIndex(
                name: "IX_UserFollows_ApplicationUserId1",
                table: "UserFollows");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "UserFollows");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "UserFollows");
        }
    }
}
