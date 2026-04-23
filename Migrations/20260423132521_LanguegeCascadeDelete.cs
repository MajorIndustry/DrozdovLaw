using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrozdovLaw.Migrations
{
    /// <inheritdoc />
    public partial class LanguegeCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Languages_LanguageCode",
                table: "Pages");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Languages_LanguageCode",
                table: "Pages",
                column: "LanguageCode",
                principalTable: "Languages",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Languages_LanguageCode",
                table: "Pages");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Languages_LanguageCode",
                table: "Pages",
                column: "LanguageCode",
                principalTable: "Languages",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
