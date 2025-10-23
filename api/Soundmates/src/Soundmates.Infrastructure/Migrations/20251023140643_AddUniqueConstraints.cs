using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soundmates.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_GiverId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Dislikes_GiverId",
                table: "Dislikes");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_GiverId_ReceiverId",
                table: "Likes",
                columns: new[] { "GiverId", "ReceiverId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dislikes_GiverId_ReceiverId",
                table: "Dislikes",
                columns: new[] { "GiverId", "ReceiverId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_GiverId_ReceiverId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Dislikes_GiverId_ReceiverId",
                table: "Dislikes");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_GiverId",
                table: "Likes",
                column: "GiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Dislikes_GiverId",
                table: "Dislikes",
                column: "GiverId");
        }
    }
}
