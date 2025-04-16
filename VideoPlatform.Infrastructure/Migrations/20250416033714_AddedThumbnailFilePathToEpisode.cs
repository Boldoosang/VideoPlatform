using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedThumbnailFilePathToEpisode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailFilePath",
                table: "Episodes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailFilePath",
                table: "Episodes");
        }
    }
}
