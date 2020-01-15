using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class UpdatedProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ethnicity",
                table: "Profile",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GraduationYear",
                table: "Profile",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ethnicity",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "GraduationYear",
                table: "Profile");
        }
    }
}
