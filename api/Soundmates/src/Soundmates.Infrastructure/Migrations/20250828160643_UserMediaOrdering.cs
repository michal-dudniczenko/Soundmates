using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soundmates.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserMediaOrdering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "ProfilePictures",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "MusicSamples",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "ProfilePictures");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "MusicSamples");
        }
    }
}
