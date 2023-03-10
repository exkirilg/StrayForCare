using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class IssueTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IssueTag",
                columns: table => new
                {
                    _issuesId = table.Column<Guid>(type: "uuid", nullable: false),
                    _tagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueTag", x => new { x._issuesId, x._tagsId });
                    table.ForeignKey(
                        name: "FK_IssueTag_Issues__issuesId",
                        column: x => x._issuesId,
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueTag_Tags__tagsId",
                        column: x => x._tagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssueTag__tagsId",
                table: "IssueTag",
                column: "_tagsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssueTag");
        }
    }
}
