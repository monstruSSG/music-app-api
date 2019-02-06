using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpotifyApi.Migrations
{
    public partial class ueragentadde : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserAgentId",
                table: "Requests",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserAgents",
                columns: table => new
                {
                    UserAgentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserAgentDescription = table.Column<string>(nullable: true),
                    SimpleSoftware = table.Column<string>(nullable: true),
                    SimpleSubDescription = table.Column<string>(nullable: true),
                    Software = table.Column<string>(nullable: true),
                    SoftwareName = table.Column<string>(nullable: true),
                    OperatingSystem = table.Column<string>(nullable: true),
                    OperatingSystemName = table.Column<string>(nullable: true),
                    OperatingSystemVersion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAgents", x => x.UserAgentId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_UserAgentId",
                table: "Requests",
                column: "UserAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_UserAgents_UserAgentId",
                table: "Requests",
                column: "UserAgentId",
                principalTable: "UserAgents",
                principalColumn: "UserAgentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_UserAgents_UserAgentId",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "UserAgents");

            migrationBuilder.DropIndex(
                name: "IX_Requests_UserAgentId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "UserAgentId",
                table: "Requests");
        }
    }
}
