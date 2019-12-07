using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class ScholarshipProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfileProperty",
                columns: table => new
                {
                    ProfilePropertyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyName = table.Column<string>(nullable: true),
                    PropertyKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileProperty", x => x.ProfilePropertyId);
                });

            migrationBuilder.CreateTable(
                name: "ScholarshipProfileProperty",
                columns: table => new
                {
                    ScholarshipId = table.Column<int>(nullable: false),
                    ProfilePropertyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScholarshipProfileProperty", x => new { x.ProfilePropertyId, x.ScholarshipId });
                    table.ForeignKey(
                        name: "FK_ScholarshipProfileProperty_ProfileProperty_ProfilePropertyId",
                        column: x => x.ProfilePropertyId,
                        principalTable: "ProfileProperty",
                        principalColumn: "ProfilePropertyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScholarshipProfileProperty_Scholarship_ScholarshipId",
                        column: x => x.ScholarshipId,
                        principalTable: "Scholarship",
                        principalColumn: "ScholarshipId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipProfileProperty_ScholarshipId",
                table: "ScholarshipProfileProperty",
                column: "ScholarshipId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScholarshipProfileProperty");

            migrationBuilder.DropTable(
                name: "ProfileProperty");
        }
    }
}
