using Microsoft.EntityFrameworkCore.Migrations;

namespace SpotifyApi.Migrations
{
    public partial class aaa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PlaylistTracks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PlaylistArtists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PlaylistAlbums");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "PlaylistTracks",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "PlaylistArtists",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "PlaylistAlbums",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "PlaylistTracks");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "PlaylistArtists");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "PlaylistAlbums");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PlaylistTracks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PlaylistArtists",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PlaylistAlbums",
                nullable: false,
                defaultValue: 0);
        }
    }
}
