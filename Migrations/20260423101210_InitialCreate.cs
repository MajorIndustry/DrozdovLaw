using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DrozdovLaw.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockStyles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockStyles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FlagImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StatusColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pages_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalTable: "Languages",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pages_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    StyleId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraAttribute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentBlocks_BlockStyles_StyleId",
                        column: x => x.StyleId,
                        principalTable: "BlockStyles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentBlocks_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BlockStyles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Заголовок 1-го уровня", "h1" },
                    { 2, "Заголовок 2-го уровня", "h2" },
                    { 3, "Заголовок 3-го уровня", "h3" },
                    { 4, "Заголовок 4-го уровня", "h4" },
                    { 5, "Заголовок 5-го уровня", "h5" },
                    { 6, "Обычный абзац", "p" },
                    { 7, "Крупный абзац", "p-large" },
                    { 8, "Цитата", "blockquote" },
                    { 9, "Мелкий текст / сноска", "small" },
                    { 10, "Маркированный список (через |)", "ul" },
                    { 11, "Нумерованный список (через |)", "ol" },
                    { 12, "Номер дела", "e-id" },
                    { 13, "Тип результата", "e-details__type" },
                    { 14, "Локация / юрисдикция", "e-details__loc" },
                    { 15, "Определение / тип дела", "e-details__def" },
                    { 16, "Заголовок пояснения", "note-title" },
                    { 17, "Текст пояснения", "note-text" },
                    { 18, "Хлебные крошки (через /)", "breadcrumb" },
                    { 19, "Заголовок секции", "section-title" },
                    { 20, "Цветные точки (цвета через ,)", "e-dots" },
                    { 21, "Адвокат", "person" },
                    { 22, "Заголовок 'Решение'", "decision-title" },
                    { 23, "Текст решения", "decision-text" },
                    { 24, "Ссылка на дело", "case-link" },
                    { 25, "Заголовок 'Документы по делу'", "docs-title" },
                    { 26, "Документ PDF", "doc-item" },
                    { 27, "Заголовок 'Теги'", "tags-title" },
                    { 28, "Тег", "tag" }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Code", "Name" },
                values: new object[,]
                {
                    { "en", "English" },
                    { "ru", "Русский" }
                });

            migrationBuilder.InsertData(
                table: "Pages",
                columns: new[] { "Id", "LanguageCode", "Name", "SectionId", "Status", "Summary", "SystemName" },
                values: new object[,]
                {
                    { 1, "ru", "Кейсы", null, null, null, "case" },
                    { 2, "en", "Cases", null, null, null, "case" }
                });

            migrationBuilder.CreateIndex(
                name: "UQ_BlockStyles_Name",
                table: "BlockStyles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentBlocks_PageId_Order",
                table: "ContentBlocks",
                columns: new[] { "PageId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_ContentBlocks_StyleId",
                table: "ContentBlocks",
                column: "StyleId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_LanguageCode",
                table: "Pages",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_SectionId",
                table: "Pages",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "UQ_Pages_SystemName_LanguageCode_SectionId",
                table: "Pages",
                columns: new[] { "SystemName", "LanguageCode", "SectionId" },
                unique: true,
                filter: "[SectionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ_Sections_Slug",
                table: "Sections",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentBlocks");

            migrationBuilder.DropTable(
                name: "BlockStyles");

            migrationBuilder.DropTable(
                name: "Pages");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Sections");
        }
    }
}
