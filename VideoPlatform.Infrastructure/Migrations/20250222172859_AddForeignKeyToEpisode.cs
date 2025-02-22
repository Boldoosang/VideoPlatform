using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyToEpisode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Seasons_SeasonId",
                table: "Episodes");

            migrationBuilder.RenameColumn(
                name: "SeasonId",
                table: "Episodes",
                newName: "seasonId");

            migrationBuilder.RenameIndex(
                name: "IX_Episodes_SeasonId",
                table: "Episodes",
                newName: "IX_Episodes_seasonId");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Episodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Seasons_seasonId",
                table: "Episodes",
                column: "seasonId",
                principalTable: "Seasons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Seasons_seasonId",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Episodes");

            migrationBuilder.RenameColumn(
                name: "seasonId",
                table: "Episodes",
                newName: "SeasonId");

            migrationBuilder.RenameIndex(
                name: "IX_Episodes_seasonId",
                table: "Episodes",
                newName: "IX_Episodes_SeasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Seasons_SeasonId",
                table: "Episodes",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "Id");
        }
    }
}
