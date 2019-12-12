using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class AddingApplicationAnswersets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Application_AnswerSet_AnswerSetId",
                table: "Application");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Application",
                table: "Application");

            migrationBuilder.DropIndex(
                name: "IX_Application_AnswerSetId",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "AnswerSetId",
                table: "Application");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "Application",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Application",
                table: "Application",
                column: "ApplicationId");

            migrationBuilder.CreateTable(
                name: "ApplicationAnswerSet",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(nullable: false),
                    AnswerSetId = table.Column<int>(nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationAnswerSet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Application",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Application");

            migrationBuilder.AddColumn<int>(
                name: "AnswerSetId",
                table: "Application",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Application",
                table: "Application",
                columns: new[] { "ProfileId", "ScholarshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_Application_AnswerSetId",
                table: "Application",
                column: "AnswerSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_AnswerSet_AnswerSetId",
                table: "Application",
                column: "AnswerSetId",
                principalTable: "AnswerSet",
                principalColumn: "AnswerSetId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
