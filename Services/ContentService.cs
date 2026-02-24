using System.Text.Json;
using DrozdovLaw.Models;

namespace DrozdovLaw.Services;

public class ContentService
{
    private readonly string _dataPath;
    private readonly ILogger<ContentService> _logger;
    private static readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ContentService(IWebHostEnvironment env, ILogger<ContentService> logger)
    {
        _logger = logger;
        _dataPath = Path.Combine(env.ContentRootPath, "Data", "content.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_dataPath)!);
    }

    public async Task<ContentData> LoadAsync()
    {
        if (!File.Exists(_dataPath)) return new ContentData();
        try
        {
            var json = await File.ReadAllTextAsync(_dataPath);
            return JsonSerializer.Deserialize<ContentData>(json, _jsonOptions) ?? new ContentData();
        }
        catch (Exception ex) { _logger.LogError(ex, "Ошибка чтения content.json"); return new ContentData(); }
    }

    public async Task SaveAsync(ContentData data)
    {
        await _lock.WaitAsync();
        try { await File.WriteAllTextAsync(_dataPath, JsonSerializer.Serialize(data, _jsonOptions)); }
        finally { _lock.Release(); }
    }

    // ── BLOCKS ──

    public async Task<List<ContentBlock>> GetPageBlocksAsync(string pageName)
    {
        var data = await LoadAsync();
        return data.Blocks.Where(b => b.PageName == pageName).OrderBy(b => b.Order).ToList();
    }

    public async Task<ContentBlock?> GetBlockAsync(string id)
    {
        var data = await LoadAsync();
        return data.Blocks.FirstOrDefault(b => b.Id == id);
    }

    public async Task SaveBlockAsync(ContentBlock block)
    {
        var data = await LoadAsync();
        var existing = data.Blocks.FirstOrDefault(b => b.Id == block.Id);
        if (existing != null)
        {
            existing.Content = block.Content; existing.Style = block.Style;
            existing.Order = block.Order; existing.ExtraAttribute = block.ExtraAttribute;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else { block.UpdatedAt = DateTime.UtcNow; data.Blocks.Add(block); }
        await SaveAsync(data);
    }

    public async Task DeleteBlockAsync(string id)
    {
        var data = await LoadAsync();
        data.Blocks.RemoveAll(b => b.Id == id);
        await SaveAsync(data);
    }

    public async Task ReorderAsync(string pageName, List<string> orderedIds)
    {
        var data = await LoadAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var b = data.Blocks.FirstOrDefault(x => x.Id == orderedIds[i] && x.PageName == pageName);
            if (b != null) b.Order = i + 1;
        }
        await SaveAsync(data);
    }

    // ── CASES ──

    public async Task<List<CaseMeta>> GetAllCasesAsync()
    {
        var data = await LoadAsync();
        return data.Cases.OrderByDescending(c => c.CreatedAt).ToList();
    }

    public async Task<CaseMeta?> GetCaseBySlugAsync(string slug)
    {
        var data = await LoadAsync();
        return data.Cases.FirstOrDefault(c => c.Slug == slug);
    }

    public async Task<CaseMeta?> GetCaseByIdAsync(string id)
    {
        var data = await LoadAsync();
        return data.Cases.FirstOrDefault(c => c.Id == id);
    }

    /// <summary>
    /// Создаёт новый кейс: метаданные + шаблонные блоки (копия структуры кейса 1)
    /// </summary>
    public async Task<CaseMeta> CreateCaseAsync(CreateCaseViewModel vm)
    {
        var data = await LoadAsync();
        var slug = vm.Slug.ToLower().Trim().Replace(" ", "-");

        var meta = new CaseMeta
        {
            Slug = slug,
            TitleRu = vm.TitleRu, TitleEn = vm.TitleEn,
            StatusColor = vm.StatusColor,
            StatusRu = vm.StatusRu, StatusEn = vm.StatusEn,
            LocationRu = vm.LocationRu, LocationEn = vm.LocationEn,
            SummaryRu = vm.SummaryRu, SummaryEn = vm.SummaryEn,
            FlagImage = vm.FlagImage,
            CreatedAt = DateTime.UtcNow, IsPublished = true
        };
        data.Cases.Add(meta);

        // Создаём шаблонные блоки (структура как у кейса 1)
        data.Blocks.AddRange(BuildTemplateBlocks($"case-{slug}-ru", slug, "ru", vm));
        data.Blocks.AddRange(BuildTemplateBlocks($"case-{slug}-en", slug, "en", vm));

        await SaveAsync(data);
        return meta;
    }

    public async Task UpdateCaseMetaAsync(CaseMeta updated)
    {
        var data = await LoadAsync();
        var existing = data.Cases.FirstOrDefault(c => c.Id == updated.Id);
        if (existing == null) return;
        existing.TitleRu = updated.TitleRu; existing.TitleEn = updated.TitleEn;
        existing.StatusColor = updated.StatusColor;
        existing.StatusRu = updated.StatusRu; existing.StatusEn = updated.StatusEn;
        existing.LocationRu = updated.LocationRu; existing.LocationEn = updated.LocationEn;
        existing.SummaryRu = updated.SummaryRu; existing.SummaryEn = updated.SummaryEn;
        existing.FlagImage = updated.FlagImage;
        existing.IsPublished = updated.IsPublished;
        await SaveAsync(data);
    }

    public async Task DeleteCaseAsync(string id)
    {
        var data = await LoadAsync();
        var meta = data.Cases.FirstOrDefault(c => c.Id == id);
        if (meta == null) return;
        // Удаляем все блоки обеих версий
        data.Blocks.RemoveAll(b => b.PageName == $"case-{meta.Slug}-ru"
                                || b.PageName == $"case-{meta.Slug}-en");
        data.Cases.Remove(meta);
        await SaveAsync(data);
    }

    // ── Шаблон блоков нового кейса (копия структуры кейса 1) ──
    private static List<ContentBlock> BuildTemplateBlocks(string pageName, string slug, string lang, CreateCaseViewModel vm)
    {
        bool ru = lang == "ru";
        var title = ru ? vm.TitleRu : vm.TitleEn;
        var location = ru ? vm.LocationRu : vm.LocationEn;
        int o = 0;
        ContentBlock B(string style, string content, string? extra = null) =>
            new() { PageName = pageName, Style = style, Content = content,
                    ExtraAttribute = extra, Order = ++o, UpdatedAt = DateTime.UtcNow };

        return new List<ContentBlock>
        {
            B("breadcrumb", ru ? "Главная / Кейсы" : "Home / Cases"),
            B("e-dots",     "#B8B399,#1E6F89"),
            B("h1",         title),
            B("e-id",       ru ? "Сообщение № —/—" : "Communication No. —/—"),
            B("e-details__type", ru ? "Статус дела" : "Case status", "#B8B399"),
            B("e-details__loc",  location, "images/flags/England.png"),
            B("e-details__def",  ru ? "Тип: укажите тип дела" : "Type: specify case type"),
            B("p",          ru ? "Введите описание дела." : "Enter case description."),
            B("h2",         ru ? "Раздел 1" : "Section 1"),
            B("p",          ru ? "Текст первого раздела." : "First section text."),
            B("blockquote", ru ? "Ключевая цитата по делу." : "Key case quote."),
            B("h2",         ru ? "Раздел 2" : "Section 2"),
            B("p",          ru ? "Текст второго раздела." : "Second section text."),
            B("h3",         ru ? "Работа по делу:" : "Work in the case:"),
            B("ul",         ru ? "Сбор информации|Подготовка документов|Защита в суде"
                               : "Information gathering|Document preparation|Court representation"),
            B("small",      ru ? "Источник: укажите источник информации."
                               : "Source: specify information source."),
            B("person",     ru ? "ДРОЗДОВ Вадим" : "DROZDOV Vadim",
                            ru ? "pictures/person-1.jpg|Адвокат" : "pictures/person-1.jpg|Lawyer"),
            B("note-title", ru ? "Пояснение" : "Explanation"),
            B("note-text",  ru ? "Добавьте пояснение к делу." : "Add case explanation."),
            B("decision-title", ru ? "Решение" : "Decision"),
            B("decision-text",  ru ? "Укажите итог дела." : "Specify case outcome."),
            B("case-link",  "https://"),
            B("docs-title", ru ? "Документы по делу" : "Case documents"),
            B("doc-item",   ru ? "Решение судебного дела" : "Court decision", ru ? "Документ PDF" : "PDF Document"),
            B("tags-title", ru ? "Теги" : "Tags"),
            B("tag",        "Tag 1", "#D5A968"),
            B("section-title", ru ? "ПОХОЖИЕ КЕЙСЫ" : "SIMILAR CASES"),
        };
    }
}
