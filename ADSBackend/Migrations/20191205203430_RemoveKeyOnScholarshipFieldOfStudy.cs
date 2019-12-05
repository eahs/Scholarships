using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class RemoveKeyOnScholarshipFieldOfStudy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_ScholarshipFieldOfStudy_ScholarshipFieldOfStudyId",
                table: "ScholarshipFieldOfStudy");

            migrationBuilder.DropColumn(
                name: "ScholarshipFieldOfStudyId",
                table: "ScholarshipFieldOfStudy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScholarshipFieldOfStudyId",
                table: "ScholarshipFieldOfStudy",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ScholarshipFieldOfStudy_ScholarshipFieldOfStudyId",
                table: "ScholarshipFieldOfStudy",
                column: "ScholarshipFieldOfStudyId");
        }
    }
}
