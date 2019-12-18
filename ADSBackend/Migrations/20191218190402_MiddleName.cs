using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class MiddleName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Profile",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Application_ProfileId",
                table: "Application",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Profile_ProfileId",
                table: "Application",
                column: "ProfileId",
                principalTable: "Profile",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Application_Profile_ProfileId",
                table: "Application");

            migrationBuilder.DropIndex(
                name: "IX_Application_ProfileId",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Profile");
        }
    }
}
