using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class CorrectedScholarshipFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistrictMaintained",
                table: "QuestionSet");

            migrationBuilder.DropColumn(
                name: "NumberOfYears",
                table: "QuestionSet");

            migrationBuilder.AddColumn<bool>(
                name: "DistrictMaintained",
                table: "Scholarship",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfYears",
                table: "Scholarship",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistrictMaintained",
                table: "Scholarship");

            migrationBuilder.DropColumn(
                name: "NumberOfYears",
                table: "Scholarship");

            migrationBuilder.AddColumn<bool>(
                name: "DistrictMaintained",
                table: "QuestionSet",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfYears",
                table: "QuestionSet",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
