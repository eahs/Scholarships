using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Scholarships.Migrations
{
    public partial class ScholarshipApplicationScoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Published",
                table: "Scholarship",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedDate",
                table: "Scholarship",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ApplicantAwardDate",
                table: "Application",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "ApplicantAwarded",
                table: "Application",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplicantFavorite",
                table: "Application",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ApplicantScore",
                table: "Application",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Published",
                table: "Scholarship");

            migrationBuilder.DropColumn(
                name: "PublishedDate",
                table: "Scholarship");

            migrationBuilder.DropColumn(
                name: "ApplicantAwardDate",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "ApplicantAwarded",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "ApplicantFavorite",
                table: "Application");

            migrationBuilder.DropColumn(
                name: "ApplicantScore",
                table: "Application");
        }
    }
}
