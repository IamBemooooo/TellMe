using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TellMe.Migrations
{
    /// <inheritdoc />
    public partial class AddEmojiField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Emoji",
                table: "MessageReactions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Emoji",
                table: "MessageReactions");
        }
    }
}
