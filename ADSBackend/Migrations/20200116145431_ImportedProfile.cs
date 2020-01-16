using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class ImportedProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImportedProfileId",
                table: "Guardian",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImportedProfile",
                columns: table => new
                {
                    ImportedProfileId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    MiddleName = table.Column<string>(nullable: true),
                    StudentId = table.Column<string>(maxLength: 5, nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    Ethnicity = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    GraduationYear = table.Column<int>(nullable: false),
                    ClassRank = table.Column<int>(nullable: true),
                    GPA = table.Column<double>(nullable: true),
                    SATScoreMath = table.Column<int>(nullable: true),
                    SATScoreReading = table.Column<int>(nullable: true),
                    ACTScore = table.Column<int>(nullable: true),
                    CollegeAttending = table.Column<string>(nullable: true),
                    TuitionYearly = table.Column<double>(nullable: false),
                    RoomBoard = table.Column<double>(nullable: false),
                    TuitionTotal = table.Column<double>(nullable: false),
                    CollegeAccepted = table.Column<bool>(nullable: false),
                    OtherColleges = table.Column<string>(nullable: true),
                    CollegeIntendedMajor = table.Column<string>(nullable: true),
                    FieldOfStudyId = table.Column<int>(nullable: true),
                    LivingSituation = table.Column<string>(nullable: true),
                    OtherAid = table.Column<string>(nullable: true),
                    ActivitiesSchool = table.Column<string>(nullable: true),
                    ActivitiesCommunity = table.Column<string>(nullable: true),
                    SchoolOffices = table.Column<string>(nullable: true),
                    SpecialCircumstances = table.Column<string>(nullable: true),
                    EarningsTotal = table.Column<double>(nullable: false),
                    FamilyAssets = table.Column<double>(nullable: false),
                    StudentEmployer = table.Column<string>(nullable: true),
                    StudentIncome = table.Column<double>(nullable: false),
                    StudentAssets = table.Column<double>(nullable: false),
                    Siblings = table.Column<string>(nullable: true),
                    FormCompletedBasic = table.Column<bool>(nullable: false),
                    FormCompletedAcademicPerformance = table.Column<bool>(nullable: false),
                    FormCompletedCollegePlans = table.Column<bool>(nullable: false),
                    FormCompletedExtraCurriculur = table.Column<bool>(nullable: false),
                    FormCompletedFamilyIncome = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportedProfile", x => x.ImportedProfileId);
                    table.ForeignKey(
                        name: "FK_ImportedProfile_FieldOfStudy_FieldOfStudyId",
                        column: x => x.FieldOfStudyId,
                        principalTable: "FieldOfStudy",
                        principalColumn: "FieldOfStudyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guardian_ImportedProfileId",
                table: "Guardian",
                column: "ImportedProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportedProfile_FieldOfStudyId",
                table: "ImportedProfile",
                column: "FieldOfStudyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guardian_ImportedProfile_ImportedProfileId",
                table: "Guardian",
                column: "ImportedProfileId",
                principalTable: "ImportedProfile",
                principalColumn: "ImportedProfileId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guardian_ImportedProfile_ImportedProfileId",
                table: "Guardian");

            migrationBuilder.DropTable(
                name: "ImportedProfile");

            migrationBuilder.DropIndex(
                name: "IX_Guardian_ImportedProfileId",
                table: "Guardian");

            migrationBuilder.DropColumn(
                name: "ImportedProfileId",
                table: "Guardian");
        }
    }
}
