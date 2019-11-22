using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class MissingQuestionQuestionSetKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_QuestionSet_QuestionSetId",
                table: "Question");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionSetId",
                table: "Question",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Question_QuestionSet_QuestionSetId",
                table: "Question",
                column: "QuestionSetId",
                principalTable: "QuestionSet",
                principalColumn: "QuestionSetId",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_QuestionSet_QuestionSetId",
                table: "Question");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionSetId",
                table: "Question",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Question_QuestionSet_QuestionSetId",
                table: "Question",
                column: "QuestionSetId",
                principalTable: "QuestionSet",
                principalColumn: "QuestionSetId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
