using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class QuestionSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionSet",
                columns: table => new
                {
                    QuestionSetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    SingularName = table.Column<string>(nullable: true),
                    PluralName = table.Column<string>(nullable: true),
                    AllowMultiple = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSet", x => x.QuestionSetId);
                });

            migrationBuilder.CreateTable(
                name: "AnswerSet",
                columns: table => new
                {
                    AnswerSetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<int>(nullable: false),
                    QuestionSetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerSet", x => x.AnswerSetId);
                    table.ForeignKey(
                        name: "FK_AnswerSet_Profile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerSet_QuestionSet_QuestionSetId",
                        column: x => x.QuestionSetId,
                        principalTable: "QuestionSet",
                        principalColumn: "QuestionSetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    QuestionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Decription = table.Column<string>(nullable: true),
                    Config = table.Column<string>(nullable: true),
                    Required = table.Column<bool>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    QuestionSetId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Question_QuestionSet_QuestionSetId",
                        column: x => x.QuestionSetId,
                        principalTable: "QuestionSet",
                        principalColumn: "QuestionSetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    AnswerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerSetId = table.Column<int>(nullable: false),
                    Response = table.Column<string>(nullable: true),
                    Config = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answer", x => x.AnswerId);
                    table.ForeignKey(
                        name: "FK_Answer_AnswerSet_AnswerSetId",
                        column: x => x.AnswerSetId,
                        principalTable: "AnswerSet",
                        principalColumn: "AnswerSetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOption",
                columns: table => new
                {
                    QuestionOptionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOption", x => x.QuestionOptionId);
                    table.ForeignKey(
                        name: "FK_QuestionOption_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerOption",
                columns: table => new
                {
                    AnswerOptionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerId = table.Column<int>(nullable: false),
                    QuestionOptionId = table.Column<int>(nullable: false)
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
                name: "IX_Answer_AnswerSetId",
                table: "Answer",
                column: "AnswerSetId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOption_AnswerId",
                table: "AnswerOption",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOption_QuestionOptionId",
                table: "AnswerOption",
                column: "QuestionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerSet_ProfileId",
                table: "AnswerSet",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerSet_QuestionSetId",
                table: "AnswerSet",
                column: "QuestionSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionSetId",
                table: "Question",
                column: "QuestionSetId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOption_QuestionId",
                table: "QuestionOption",
                column: "QuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerOption");

            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.DropTable(
                name: "QuestionOption");

            migrationBuilder.DropTable(
                name: "AnswerSet");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "QuestionSet");
        }
    }
}
