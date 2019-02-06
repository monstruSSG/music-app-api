using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpotifyApi.Migrations
{
    public partial class aaa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlaylistAlbums",
                columns: table => new
                {
                    PlaylistAlbumId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    ImgUri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistAlbums", x => x.PlaylistAlbumId);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistArtists",
                columns: table => new
                {
                    PlaylistArtistId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true),
                    ImgUri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistArtists", x => x.PlaylistArtistId);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistTracks",
                columns: table => new
                {
                    PlaylistTrackId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Href = table.Column<string>(nullable: true),
                    PreviewUrl = table.Column<string>(nullable: true),
                    PlaylistAlbumId = table.Column<int>(nullable: true),
                    PlaylistArtistId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistTracks", x => x.PlaylistTrackId);
                    table.ForeignKey(
                        name: "FK_PlaylistTracks_PlaylistAlbums_PlaylistAlbumId",
                        column: x => x.PlaylistAlbumId,
                        principalTable: "PlaylistAlbums",
                        principalColumn: "PlaylistAlbumId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlaylistTracks_PlaylistArtists_PlaylistArtistId",
                        column: x => x.PlaylistArtistId,
                        principalTable: "PlaylistArtists",
                        principalColumn: "PlaylistArtistId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistTracks_PlaylistAlbumId",
                table: "PlaylistTracks",
                column: "PlaylistAlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistTracks_PlaylistArtistId",
                table: "PlaylistTracks",
                column: "PlaylistArtistId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaylistTracks");

            migrationBuilder.DropTable(
                name: "PlaylistAlbums");

            migrationBuilder.DropTable(
                name: "PlaylistArtists");
        }
    }
}
