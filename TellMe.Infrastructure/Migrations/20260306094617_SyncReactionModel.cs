using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TellMe.Migrations
{
    /// <inheritdoc />
    public partial class SyncReactionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MessageReactions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "MessageReactions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MessageReactions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MessageReactions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "MessageReactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "MessageReactions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "MessageReactions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MessageReactions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MessageReactions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "MessageReactions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }
    }
}
