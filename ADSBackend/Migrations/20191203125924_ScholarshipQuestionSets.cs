using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class ScholarshipQuestionSets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionSetId",
                table: "Scholarship",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Scholarship_QuestionSetId",
                table: "Scholarship",
                column: "QuestionSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scholarship_QuestionSet_QuestionSetId",
                table: "Scholarship",
                column: "QuestionSetId",
                principalTable: "QuestionSet",
                principalColumn: "QuestionSetId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scholarship_QuestionSet_QuestionSetId",
                table: "Scholarship");

            migrationBuilder.DropIndex(
                name: "IX_Scholarship_QuestionSetId",
                table: "Scholarship");

            migrationBuilder.DropColumn(
                name: "QuestionSetId",
                table: "Scholarship");
        }
    }
}
