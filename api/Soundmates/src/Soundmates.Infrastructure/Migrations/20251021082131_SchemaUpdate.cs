using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soundmates.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SchemaUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bands_UserId",
                table: "Bands");

            migrationBuilder.DropIndex(
                name: "IX_Artists_UserId",
                table: "Artists");

            migrationBuilder.CreateIndex(
                name: "IX_Bands_UserId",
                table: "Bands",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artists_UserId",
                table: "Artists",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bands_UserId",
                table: "Bands");

            migrationBuilder.DropIndex(
                name: "IX_Artists_UserId",
                table: "Artists");

            migrationBuilder.CreateIndex(
                name: "IX_Bands_UserId",
                table: "Bands",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Artists_UserId",
                table: "Artists",
                column: "UserId");
        }
    }
}
