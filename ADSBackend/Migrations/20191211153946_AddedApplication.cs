using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class AddedApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    ProfileId = table.Column<int>(nullable: false),
                    ScholarshipId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    SubmittedDate = table.Column<DateTime>(nullable: false),
                    SubmissionStage = table.Column<int>(nullable: false),
                    Submitted = table.Column<bool>(nullable: false),
                    AnswerSetId = table.Column<int>(nullable: false),
                    ProfileSnapshot = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => new { x.ProfileId, x.ScholarshipId });
                    table.ForeignKey(
                        name: "FK_Application_AnswerSet_AnswerSetId",
                        column: x => x.AnswerSetId,
                        principalTable: "AnswerSet",
                        principalColumn: "AnswerSetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application_AnswerSetId",
                table: "Application",
                column: "AnswerSetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application");
        }
    }
}
