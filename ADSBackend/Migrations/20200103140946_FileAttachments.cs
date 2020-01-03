using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scholarships.Migrations
{
    public partial class FileAttachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileAttachmentGroupId",
                table: "Answer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FileAttachmentGroup",
                columns: table => new
                {
                    FileAttachmentGroupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachmentGroup", x => x.FileAttachmentGroupId);
                });

            migrationBuilder.CreateTable(
                name: "FileAttachment",
                columns: table => new
                {
                    FileAttachmentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileAttachmentGroupId = table.Column<int>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_Answer_FileAttachmentGroupId",
                table: "Answer",
                column: "FileAttachmentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachment_FileAttachmentGroupId",
                table: "FileAttachment",
                column: "FileAttachmentGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_FileAttachmentGroup_FileAttachmentGroupId",
                table: "Answer",
                column: "FileAttachmentGroupId",
                principalTable: "FileAttachmentGroup",
                principalColumn: "FileAttachmentGroupId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_FileAttachmentGroup_FileAttachmentGroupId",
                table: "Answer");

            migrationBuilder.DropTable(
                name: "FileAttachment");

            migrationBuilder.DropTable(
                name: "FileAttachmentGroup");

            migrationBuilder.DropIndex(
                name: "IX_Answer_FileAttachmentGroupId",
                table: "Answer");

            migrationBuilder.DropColumn(
                name: "FileAttachmentGroupId",
                table: "Answer");
        }
    }
}
