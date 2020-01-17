using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Scholarships.Migrations
{
    public partial class ScholarshipFavorites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FormCompletedExtraCurriculur", 
                newName: "FormCompletedExtraCurricular", 
                table: "Profile");

            migrationBuilder.CreateTable(
                name: "ScholarshipFavorite",
                columns: table => new
                {
                    ScholarshipId = table.Column<int>(nullable: false),
                    ProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScholarshipFavorite", x => new { x.ProfileId, x.ScholarshipId });
                    table.ForeignKey(
                        name: "FK_ScholarshipFavorite_Profile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScholarshipFavorite_Scholarship_ScholarshipId",
                        column: x => x.ScholarshipId,
                        principalTable: "Scholarship",
                        principalColumn: "ScholarshipId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipFavorite_ScholarshipId",
                table: "ScholarshipFavorite",
                column: "ScholarshipId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScholarshipFavorite");

        }
    }
}
