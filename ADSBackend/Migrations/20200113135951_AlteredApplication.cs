using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class AlteredApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_ScholarshipCategory_ScholarshipCategoryId",
                table: "ScholarshipCategory");

            migrationBuilder.AddColumn<bool>(
                name: "FerpaTerms",
                table: "Application",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "Application",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignatureDate",
                table: "Application",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FerpaTerms",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "Signature",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "SignatureDate",
                table: "Application");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ScholarshipCategory_ScholarshipCategoryId",
                table: "ScholarshipCategory",
                column: "ScholarshipCategoryId");
        }
    }
}
