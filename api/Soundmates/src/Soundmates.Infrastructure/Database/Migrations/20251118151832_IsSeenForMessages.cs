using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soundmates.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class IsSeenForMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSeen",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSeen",
                table: "Messages");
        }
    }
}
