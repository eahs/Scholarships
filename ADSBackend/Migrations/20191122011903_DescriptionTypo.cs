using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class DescriptionTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Decription",
                table: "Question");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Question",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Question");

            migrationBuilder.AddColumn<string>(
                name: "Decription",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
