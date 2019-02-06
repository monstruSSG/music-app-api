using Microsoft.EntityFrameworkCore.Migrations;

namespace SpotifyApi.Migrations
{
    public partial class AddedSurcetRequet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Link",
                table: "Requests",
                newName: "Source");

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "Requests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destination",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "Source",
                table: "Requests",
                newName: "Link");
        }
    }
}
