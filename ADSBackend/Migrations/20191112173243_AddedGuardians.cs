using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class AddedGuardians : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EarningsFather",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "EarningsMother",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "FatherEmployer",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "FatherName",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "FatherOccupation",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "MotherEmployer",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "MotherName",
                table: "Profile");

            migrationBuilder.RenameColumn(
                name: "MotherOccupation",
                table: "Profile",
                newName: "State");

            migrationBuilder.AlterColumn<string>(
                name: "TuitionYearly",
                table: "Profile",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<string>(
                name: "TuitionTotal",
                table: "Profile",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Profile",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SATScoreReading",
                table: "Profile",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "SATScoreMath",
                table: "Profile",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "RoomBoard",
                table: "Profile",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Profile",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Profile",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Profile",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClassRank",
                table: "Profile",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ACTScore",
                table: "Profile",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<bool>(
                name: "FormCompletedAcademicPerformance",
                table: "Profile",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FormCompletedBasic",
                table: "Profile",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FormCompletedCollegePlans",
                table: "Profile",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FormCompletedExtraCurriculur",
                table: "Profile",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FormCompletedFamilyIncome",
                table: "Profile",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "GPA",
                table: "Profile",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Category",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Category",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Guardian",
                columns: table => new
                {
                    GuardianId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProfileId = table.Column<int>(nullable: false),
                    FullName = table.Column<int>(nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_Guardian_ProfileId",
                table: "Guardian",
                column: "ProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guardian");

            migrationBuilder.DropColumn(
                name: "FormCompletedAcademicPerformance",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "FormCompletedBasic",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "FormCompletedCollegePlans",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "FormCompletedExtraCurriculur",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "FormCompletedFamilyIncome",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "GPA",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Category");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Profile",
                newName: "MotherOccupation");

            migrationBuilder.AlterColumn<double>(
                name: "TuitionYearly",
                table: "Profile",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TuitionTotal",
                table: "Profile",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Profile",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<int>(
                name: "SATScoreReading",
                table: "Profile",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SATScoreMath",
                table: "Profile",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "RoomBoard",
                table: "Profile",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Profile",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Profile",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Profile",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "ClassRank",
                table: "Profile",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ACTScore",
                table: "Profile",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EarningsFather",
                table: "Profile",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EarningsMother",
                table: "Profile",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "FatherEmployer",
                table: "Profile",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FatherName",
                table: "Profile",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FatherOccupation",
                table: "Profile",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotherEmployer",
                table: "Profile",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotherName",
                table: "Profile",
                nullable: true);
        }
    }
}
