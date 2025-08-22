using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HandiCraft.Presistance.context.Migrations
{
    /// <inheritdoc />
    public partial class deleteTypefromReact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Reactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Reactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
