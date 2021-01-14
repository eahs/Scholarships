using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Scholarships.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnswerGroup",
                columns: table => new
                {
                    AnswerGroupId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerGroup", x => x.AnswerGroupId);
                });

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    ArticleId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Lead = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    PublishDate = table.Column<DateTime>(nullable: false),
                    Published = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.ArticleId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationItem",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationItem", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "FieldOfStudy",
                columns: table => new
                {
                    FieldOfStudyId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldOfStudy", x => x.FieldOfStudyId);
                });

            migrationBuilder.CreateTable(
                name: "FileAttachmentGroup",
                columns: table => new
                {
                    FileAttachmentGroupId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachmentGroup", x => x.FileAttachmentGroupId);
                });

            migrationBuilder.CreateTable(
                name: "ProfileProperty",
                columns: table => new
                {
                    ProfilePropertyId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PropertyName = table.Column<string>(nullable: true),
                    PropertyKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileProperty", x => x.ProfilePropertyId);
                });

            migrationBuilder.CreateTable(
                name: "QuestionSet",
                columns: table => new
                {
                    QuestionSetId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    SingularName = table.Column<string>(nullable: false),
                    PluralName = table.Column<string>(nullable: false),
                    AllowMultiple = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSet", x => x.QuestionSetId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    ProfileId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    MiddleName = table.Column<string>(nullable: true),
                    StudentId = table.Column<string>(maxLength: 5, nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
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
                    table.PrimaryKey("PK_Profile", x => x.ProfileId);
                    table.ForeignKey(
                        name: "FK_Profile_FieldOfStudy_FieldOfStudyId",
                        column: x => x.FieldOfStudyId,
                        principalTable: "FieldOfStudy",
                        principalColumn: "FieldOfStudyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Profile_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileAttachment",
                columns: table => new
                {
                    FileAttachmentId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileAttachmentGroupId = table.Column<int>(nullable: false),
                    FileAttachmentUuid = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    FileSubPath = table.Column<string>(nullable: true),
                    Length = table.Column<long>(nullable: false),
                    SecureFileName = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachment", x => x.FileAttachmentId);
                    table.ForeignKey(
                        name: "FK_FileAttachment_FileAttachmentGroup_FileAttachmentGroupId",
                        column: x => x.FileAttachmentGroupId,
                        principalTable: "FileAttachmentGroup",
                        principalColumn: "FileAttachmentGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    QuestionId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    QuestionSetId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Config = table.Column<string>(nullable: true),
                    Required = table.Column<bool>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Question_QuestionSet_QuestionSetId",
                        column: x => x.QuestionSetId,
                        principalTable: "QuestionSet",
                        principalColumn: "QuestionSetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scholarship",
                columns: table => new
                {
                    ScholarshipId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SponsorCompany = table.Column<string>(nullable: true),
                    SponsorName = table.Column<string>(nullable: false),
                    SponsorAddress1 = table.Column<string>(nullable: true),
                    SponsorAddress2 = table.Column<string>(nullable: true),
                    SponsorPhone = table.Column<string>(nullable: true),
                    SponsorEmail = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Eligibility = table.Column<string>(nullable: true),
                    Standards = table.Column<string>(nullable: true),
                    Amount = table.Column<string>(nullable: true),
                    ApplicationInstructions = table.Column<string>(nullable: true),
                    ApplyOnline = table.Column<bool>(nullable: false),
                    TranscriptsRequired = table.Column<bool>(nullable: false),
                    IncomeVerificationRequired = table.Column<bool>(nullable: false),
                    DistrictMaintained = table.Column<bool>(nullable: false),
                    NumberOfYears = table.Column<int>(nullable: false),
                    ReleaseDate = table.Column<DateTime>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    QuestionSetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scholarship", x => x.ScholarshipId);
                    table.ForeignKey(
                        name: "FK_Scholarship_QuestionSet_QuestionSetId",
                        column: x => x.QuestionSetId,
                        principalTable: "QuestionSet",
                        principalColumn: "QuestionSetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerSet",
                columns: table => new
                {
                    AnswerSetId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProfileId = table.Column<int>(nullable: false),
                    QuestionSetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerSet", x => x.AnswerSetId);
                    table.ForeignKey(
                        name: "FK_AnswerSet_Profile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerSet_QuestionSet_QuestionSetId",
                        column: x => x.QuestionSetId,
                        principalTable: "QuestionSet",
                        principalColumn: "QuestionSetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProfileId = table.Column<int>(nullable: false),
                    ScholarshipId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    SubmittedDate = table.Column<DateTime>(nullable: false),
                    SubmissionStage = table.Column<int>(nullable: false),
                    Submitted = table.Column<bool>(nullable: false),
                    Signature = table.Column<string>(nullable: true),
                    AcceptRelease = table.Column<bool>(nullable: false),
                    FerpaTerms = table.Column<bool>(nullable: false),
                    SignatureDate = table.Column<DateTime>(nullable: false),
                    AnswerGroupId = table.Column<int>(nullable: false),
                    ProfileSnapshot = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_Application_AnswerGroup_AnswerGroupId",
                        column: x => x.AnswerGroupId,
                        principalTable: "AnswerGroup",
                        principalColumn: "AnswerGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Application_Profile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guardian",
                columns: table => new
                {
                    GuardianId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProfileId = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(nullable: false),
                    Relationship = table.Column<int>(nullable: false),
                    EmploymentStatus = table.Column<int>(nullable: false),
                    Occupation = table.Column<string>(nullable: true),
                    Employer = table.Column<string>(nullable: true),
                    AnnualIncome = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guardian", x => x.GuardianId);
                    table.ForeignKey(
                        name: "FK_Guardian_Profile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOption",
                columns: table => new
                {
                    QuestionOptionId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    QuestionId = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOption", x => x.QuestionOptionId);
                    table.ForeignKey(
                        name: "FK_QuestionOption_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "ScholarshipFieldOfStudy",
                columns: table => new
                {
                    ScholarshipId = table.Column<int>(nullable: false),
                    FieldOfStudyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScholarshipFieldOfStudy", x => new { x.FieldOfStudyId, x.ScholarshipId });
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

            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    AnswerId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AnswerSetId = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    Response = table.Column<string>(nullable: true),
                    QuestionOptionId = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Config = table.Column<string>(nullable: true),
                    FileAttachmentGroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answer", x => x.AnswerId);
                    table.ForeignKey(
                        name: "FK_Answer_AnswerSet_AnswerSetId",
                        column: x => x.AnswerSetId,
                        principalTable: "AnswerSet",
                        principalColumn: "AnswerSetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Answer_FileAttachmentGroup_FileAttachmentGroupId",
                        column: x => x.FileAttachmentGroupId,
                        principalTable: "FileAttachmentGroup",
                        principalColumn: "FileAttachmentGroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Answer_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerGroupSets",
                columns: table => new
                {
                    AnswerGroupId = table.Column<int>(nullable: false),
                    AnswerSetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerGroupSets", x => new { x.AnswerSetId, x.AnswerGroupId });
                    table.ForeignKey(
                        name: "FK_AnswerGroupSets_AnswerGroup_AnswerGroupId",
                        column: x => x.AnswerGroupId,
                        principalTable: "AnswerGroup",
                        principalColumn: "AnswerGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerGroupSets_AnswerSet_AnswerSetId",
                        column: x => x.AnswerSetId,
                        principalTable: "AnswerSet",
                        principalColumn: "AnswerSetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answer_AnswerSetId",
                table: "Answer",
                column: "AnswerSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_FileAttachmentGroupId",
                table: "Answer",
                column: "FileAttachmentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_QuestionId",
                table: "Answer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerGroupSets_AnswerGroupId",
                table: "AnswerGroupSets",
                column: "AnswerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerSet_ProfileId",
                table: "AnswerSet",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerSet_QuestionSetId",
                table: "AnswerSet",
                column: "QuestionSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Application_AnswerGroupId",
                table: "Application",
                column: "AnswerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Application_ProfileId",
                table: "Application",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachment_FileAttachmentGroupId",
                table: "FileAttachment",
                column: "FileAttachmentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Guardian_ProfileId",
                table: "Guardian",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_FieldOfStudyId",
                table: "Profile",
                column: "FieldOfStudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_UserId",
                table: "Profile",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionSetId",
                table: "Question",
                column: "QuestionSetId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOption_QuestionId",
                table: "QuestionOption",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Scholarship_QuestionSetId",
                table: "Scholarship",
                column: "QuestionSetId");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipCategory_ScholarshipId",
                table: "ScholarshipCategory",
                column: "ScholarshipId");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipFieldOfStudy_ScholarshipId",
                table: "ScholarshipFieldOfStudy",
                column: "ScholarshipId");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipProfileProperty_ScholarshipId",
                table: "ScholarshipProfileProperty",
                column: "ScholarshipId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.DropTable(
                name: "AnswerGroupSets");

            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ConfigurationItem");

            migrationBuilder.DropTable(
                name: "FileAttachment");

            migrationBuilder.DropTable(
                name: "Guardian");

            migrationBuilder.DropTable(
                name: "QuestionOption");

            migrationBuilder.DropTable(
                name: "ScholarshipCategory");

            migrationBuilder.DropTable(
                name: "ScholarshipFieldOfStudy");

            migrationBuilder.DropTable(
                name: "ScholarshipProfileProperty");

            migrationBuilder.DropTable(
                name: "AnswerSet");

            migrationBuilder.DropTable(
                name: "AnswerGroup");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "FileAttachmentGroup");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "ProfileProperty");

            migrationBuilder.DropTable(
                name: "Scholarship");

            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "QuestionSet");

            migrationBuilder.DropTable(
                name: "FieldOfStudy");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
