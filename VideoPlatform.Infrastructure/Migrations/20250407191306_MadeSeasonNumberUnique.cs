using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MadeSeasonNumberUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Seasons_SeasonNumber",
                table: "Seasons",
                column: "SeasonNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seasons_SeasonNumber",
                table: "Seasons");
        }
    }
}
