using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class AppScholarshipRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Application_ScholarshipId",
                table: "Application",
                column: "ScholarshipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Scholarship_ScholarshipId",
                table: "Application",
                column: "ScholarshipId",
                principalTable: "Scholarship",
                principalColumn: "ScholarshipId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Application_Scholarship_ScholarshipId",
                table: "Application");

            migrationBuilder.DropIndex(
                name: "IX_Application_ScholarshipId",
                table: "Application");
        }
    }
}
