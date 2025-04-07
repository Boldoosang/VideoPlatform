using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublishedStateToSeason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Seasons",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Seasons");
        }
    }
}
