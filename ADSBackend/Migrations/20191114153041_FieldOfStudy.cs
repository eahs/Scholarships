using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class FieldOfStudy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FieldOfStudyId",
                table: "Profile",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FieldOfStudy",
                columns: table => new
                {
                    FieldOfStudyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldOfStudy", x => x.FieldOfStudyId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profile_FieldOfStudyId",
                table: "Profile",
                column: "FieldOfStudyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Profile_FieldOfStudy_FieldOfStudyId",
                table: "Profile",
                column: "FieldOfStudyId",
                principalTable: "FieldOfStudy",
                principalColumn: "FieldOfStudyId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profile_FieldOfStudy_FieldOfStudyId",
                table: "Profile");

            migrationBuilder.DropTable(
                name: "FieldOfStudy");

            migrationBuilder.DropIndex(
                name: "IX_Profile_FieldOfStudyId",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "FieldOfStudyId",
                table: "Profile");
        }
    }
}
