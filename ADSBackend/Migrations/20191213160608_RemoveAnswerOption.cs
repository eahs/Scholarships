using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class RemoveAnswerOption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerOption");

            migrationBuilder.AddColumn<int>(
                name: "QuestionOptionId",
                table: "Answer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionOptionId",
                table: "Answer");

            migrationBuilder.CreateTable(
                name: "AnswerOption",
                columns: table => new
                {
                    AnswerOptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerId = table.Column<int>(type: "int", nullable: false),
                    QuestionOptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerOption", x => x.AnswerOptionId);
                    table.ForeignKey(
                        name: "FK_AnswerOption_Answer_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answer",
                        principalColumn: "AnswerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerOption_QuestionOption_QuestionOptionId",
                        column: x => x.QuestionOptionId,
                        principalTable: "QuestionOption",
                        principalColumn: "QuestionOptionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOption_AnswerId",
                table: "AnswerOption",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOption_QuestionOptionId",
                table: "AnswerOption",
                column: "QuestionOptionId");
        }
    }
}
