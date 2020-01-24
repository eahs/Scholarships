using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class LunchStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LunchStatus",
                table: "Profile",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LunchStatus",
                table: "ImportedProfile",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LunchStatus",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "LunchStatus",
                table: "ImportedProfile");
        }
    }
}
