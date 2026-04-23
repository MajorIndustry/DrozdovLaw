using System.Text.Json;
using DrozdovLaw.Data;
using DrozdovLaw.Models;
using Microsoft.EntityFrameworkCore;

public class JsonMigrator
{
    private readonly AppDbContext _db;

    public JsonMigrator(AppDbContext db) => _db = db;

    public async Task MigrateAsync(string jsonFilePath)
    {
        var json = await File.ReadAllTextAsync(jsonFilePath);
        var data = JsonSerializer.Deserialize<LegacyContentData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Сначала сохраняем Section и Page
        foreach (var c in data.Cases)
        {
            var section = new Section
            {
                Slug = c.Slug,
                FlagImage = c.FlagImage,
                StatusColor = c.StatusColor,
                CreatedAt = c.CreatedAt,
                IsPublished = c.IsPublished
            };
            _db.Sections.Add(section);
            await _db.SaveChangesAsync();

            // Создаём страницы для ru и en
            var pageRu = new Page
            {
                SystemName = "case",
                Name=c.TitleRu,
                LanguageCode = "ru",
                SectionId = section.Id,
                Status = c.StatusRu,
                Summary = c.SummaryRu
            };
            var pageEn = new Page
            {
                SystemName = "case",
                Name = c.TitleEn,
                LanguageCode = "en",
                SectionId = section.Id,
                Status = c.StatusEn,
                Summary = c.SummaryEn
            };
            _db.Pages.AddRange(pageRu, pageEn);
            await _db.SaveChangesAsync();

            // Блоки для ru
            var ruBlocks = data.Blocks.Where(b => b.PageName == $"case-{c.Slug}-ru");
            await AddBlocksAsync(ruBlocks, pageRu.Id);

            // Блоки для en
            var enBlocks = data.Blocks.Where(b => b.PageName == $"case-{c.Slug}-en");
            await AddBlocksAsync(enBlocks, pageEn.Id);
        }

        // Статические страницы whoweare
        await MigrateStaticPageAsync(data, "whoweare-ru", "ru");
        await MigrateStaticPageAsync(data, "whoweare-en", "en");
    }

    private async Task AddBlocksAsync(IEnumerable<LegacyBlock> blocks, int pageId)
    {
        var styles = await _db.BlockStyles.ToDictionaryAsync(s => s.Name);
        foreach (var b in blocks.OrderBy(b => b.Order))
        {
            if (!styles.TryGetValue(b.Style, out var style))
                continue;

            var block = new ContentBlock
            {
                PageId = pageId,
                Order = b.Order,
                StyleId = style.Id,
                Content = b.Content,
                ExtraAttribute = b.ExtraAttribute,
                UpdatedAt = b.UpdatedAt
            };
            _db.ContentBlocks.Add(block);
        }
        await _db.SaveChangesAsync();
    }

    private async Task MigrateStaticPageAsync(LegacyContentData data, string pageName, string lang)
    {
        var page = await _db.Pages.FirstOrDefaultAsync(p => p.Name == "whoweare" && p.LanguageCode == lang);
        if (page == null)
        {
            page = new Page
            {

                SystemName = "whoweare",
                Name = pageName,
                LanguageCode = lang,
            };
            _db.Pages.Add(page);
            await _db.SaveChangesAsync();
        }

        var blocks = data.Blocks.Where(b => b.PageName == pageName);
        await AddBlocksAsync(blocks, page.Id);
    }

    // Вспомогательные классы для десериализации
    public class LegacyContentData
    {
        public List<LegacyBlock> Blocks { get; set; }
        public List<LegacyCase> Cases { get; set; }
    }

    public class LegacyBlock
    {
        public string PageName { get; set; }
        public int Order { get; set; }
        public string Style { get; set; }
        public string Content { get; set; }
        public string ExtraAttribute { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class LegacyCase
    {
        public string Slug { get; set; }
        public string TitleRu { get; set; }
        public string TitleEn { get; set; }
        public string StatusColor { get; set; }
        public string StatusRu { get; set; }
        public string StatusEn { get; set; }
        public string LocationRu { get; set; }
        public string LocationEn { get; set; }
        public string SummaryRu { get; set; }
        public string SummaryEn { get; set; }
        public string FlagImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPublished { get; set; }
    }
}