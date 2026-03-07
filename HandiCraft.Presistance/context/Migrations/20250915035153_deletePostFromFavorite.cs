using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandiCraft.Presistance.context.Migrations
{
    /// <inheritdoc />
    public partial class deletePostFromFavorite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteLists_Posts_PostId",
                table: "FavoriteLists");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteLists_PostId",
                table: "FavoriteLists");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "FavoriteLists");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PostId",
                table: "FavoriteLists",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteLists_PostId",
                table: "FavoriteLists",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteLists_Posts_PostId",
                table: "FavoriteLists",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
