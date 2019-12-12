using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class AddedAnswerGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationAnswerSet");

            migrationBuilder.AddColumn<bool>(
                name: "DistrictMaintained",
                table: "QuestionSet",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfYears",
                table: "QuestionSet",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AnswerGroupId",
                table: "Application",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AnswerGroup",
                columns: table => new
                {
                    AnswerGroupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerGroup", x => x.AnswerGroupId);
                });

            migrationBuilder.CreateTable(
                name: "AnswerGroupSets",
                columns: table => new
                {
                    AnswerGroupId = table.Column<int>(nullable: false),
                    AnswerSetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerGroupSets", x => new { x.AnswerSetId, x.AnswerGroupId });
                    table.ForeignKey(
                        name: "FK_AnswerGroupSets_AnswerGroup_AnswerGroupId",
                        column: x => x.AnswerGroupId,
                        principalTable: "AnswerGroup",
                        principalColumn: "AnswerGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerGroupSets_AnswerSet_AnswerSetId",
                        column: x => x.AnswerSetId,
                        principalTable: "AnswerSet",
                        principalColumn: "AnswerSetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application_AnswerGroupId",
                table: "Application",
                column: "AnswerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerGroupSets_AnswerGroupId",
                table: "AnswerGroupSets",
                column: "AnswerGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_AnswerGroup_AnswerGroupId",
                table: "Application",
                column: "AnswerGroupId",
                principalTable: "AnswerGroup",
                principalColumn: "AnswerGroupId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Application_AnswerGroup_AnswerGroupId",
                table: "Application");

            migrationBuilder.DropTable(
                name: "AnswerGroupSets");

            migrationBuilder.DropTable(
                name: "AnswerGroup");

            migrationBuilder.DropIndex(
                name: "IX_Application_AnswerGroupId",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "DistrictMaintained",
                table: "QuestionSet");

            migrationBuilder.DropColumn(
                name: "NumberOfYears",
                table: "QuestionSet");

            migrationBuilder.DropColumn(
                name: "AnswerGroupId",
                table: "Application");

            migrationBuilder.CreateTable(
                name: "ApplicationAnswerSet",
                columns: table => new
                {
                    AnswerSetId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationAnswerSet", x => new { x.AnswerSetId, x.ApplicationId });
                    table.ForeignKey(
                        name: "FK_ApplicationAnswerSet_AnswerSet_AnswerSetId",
                        column: x => x.AnswerSetId,
                        principalTable: "AnswerSet",
                        principalColumn: "AnswerSetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationAnswerSet_Application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationAnswerSet_ApplicationId",
                table: "ApplicationAnswerSet",
                column: "ApplicationId");
        }
    }
}
