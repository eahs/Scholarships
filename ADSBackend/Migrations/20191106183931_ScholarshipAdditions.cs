using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class ScholarshipAdditions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    ProfileId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    StudentId = table.Column<string>(nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    ClassRank = table.Column<int>(nullable: false),
                    SATScoreMath = table.Column<int>(nullable: false),
                    SATScoreReading = table.Column<int>(nullable: false),
                    ACTScore = table.Column<int>(nullable: false),
                    CollegeAttending = table.Column<string>(nullable: true),
                    TuitionYearly = table.Column<double>(nullable: false),
                    RoomBoard = table.Column<double>(nullable: false),
                    TuitionTotal = table.Column<double>(nullable: false),
                    CollegeAccepted = table.Column<bool>(nullable: false),
                    CollegeIntendedMajor = table.Column<string>(nullable: true),
                    LivingSituation = table.Column<string>(nullable: true),
                    OtherAid = table.Column<string>(nullable: true),
                    ActivitiesSchool = table.Column<string>(nullable: true),
                    ActivitiesCommunity = table.Column<string>(nullable: true),
                    SchoolOffices = table.Column<string>(nullable: true),
                    SpecialCircumstances = table.Column<string>(nullable: true),
                    FatherName = table.Column<string>(nullable: true),
                    FatherOccupation = table.Column<string>(nullable: true),
                    FatherEmployer = table.Column<string>(nullable: true),
                    MotherName = table.Column<string>(nullable: true),
                    MotherOccupation = table.Column<string>(nullable: true),
                    MotherEmployer = table.Column<string>(nullable: true),
                    EarningsFather = table.Column<string>(nullable: true),
                    EarningsMother = table.Column<double>(nullable: false),
                    EarningsTotal = table.Column<double>(nullable: false),
                    FamilyAssets = table.Column<double>(nullable: false),
                    StudentEmployer = table.Column<string>(nullable: true),
                    StudentIncome = table.Column<double>(nullable: false),
                    StudentAssets = table.Column<double>(nullable: false),
                    Siblings = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.ProfileId);
                });

            migrationBuilder.CreateTable(
                name: "Scholarship",
                columns: table => new
                {
                    ScholarshipId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SponsorCompany = table.Column<string>(nullable: true),
                    SponsorName = table.Column<string>(nullable: true),
                    SponsorAddress1 = table.Column<string>(nullable: true),
                    SponsorAddress2 = table.Column<string>(nullable: true),
                    SponsorPhone = table.Column<string>(nullable: true),
                    SponsorEmail = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Eligibility = table.Column<string>(nullable: true),
                    Standards = table.Column<string>(nullable: true),
                    Amount = table.Column<string>(nullable: true),
                    ApplicationInstructions = table.Column<string>(nullable: true),
                    ApplyOnline = table.Column<bool>(nullable: false),
                    TranscriptsRequired = table.Column<bool>(nullable: false),
                    ReleaseDate = table.Column<DateTime>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scholarship", x => x.ScholarshipId);
                });

            migrationBuilder.CreateTable(
                name: "ScholarshipCategory",
                columns: table => new
                {
                    ScholarshipId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    ScholarshipCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScholarshipCategory", x => new { x.CategoryId, x.ScholarshipId });
                    table.UniqueConstraint("AK_ScholarshipCategory_ScholarshipCategoryId", x => x.ScholarshipCategoryId);
                    table.ForeignKey(
                        name: "FK_ScholarshipCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScholarshipCategory_Scholarship_ScholarshipId",
                        column: x => x.ScholarshipId,
                        principalTable: "Scholarship",
                        principalColumn: "ScholarshipId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipCategory_ScholarshipId",
                table: "ScholarshipCategory",
                column: "ScholarshipId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "ScholarshipCategory");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Scholarship");
        }
    }
}
