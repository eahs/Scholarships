using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class FieldOfStudyScholarshipRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScholarshipFieldOfStudy",
                columns: table => new
                {
                    ScholarshipId = table.Column<int>(nullable: false),
                    FieldOfStudyId = table.Column<int>(nullable: false),
                    ScholarshipFieldOfStudyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScholarshipFieldOfStudy", x => new { x.FieldOfStudyId, x.ScholarshipId });
                    table.UniqueConstraint("AK_ScholarshipFieldOfStudy_ScholarshipFieldOfStudyId", x => x.ScholarshipFieldOfStudyId);
                    table.ForeignKey(
                        name: "FK_ScholarshipFieldOfStudy_FieldOfStudy_FieldOfStudyId",
                        column: x => x.FieldOfStudyId,
                        principalTable: "FieldOfStudy",
                        principalColumn: "FieldOfStudyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScholarshipFieldOfStudy_Scholarship_ScholarshipId",
                        column: x => x.ScholarshipId,
                        principalTable: "Scholarship",
                        principalColumn: "ScholarshipId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipFieldOfStudy_ScholarshipId",
                table: "ScholarshipFieldOfStudy",
                column: "ScholarshipId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScholarshipFieldOfStudy");
        }
    }
}
