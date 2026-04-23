using DrozdovLaw.Data;
using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;
using DrozdovLaw.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DrozdovLaw.Services;

public class SectionService : ISectionService
{
    private readonly AppDbContext _db;
    private readonly IBlockService _blockService;
    private readonly ICaseTemplateBuilder _templateBuilder;
    private readonly ITranslationService _translationService;

    public SectionService(AppDbContext db, IBlockService blockService, ICaseTemplateBuilder templateBuilder, ITranslationService translationService)
    {
        _db = db;
        _blockService = blockService;
        _templateBuilder = templateBuilder;
        _translationService = translationService;
    }

    public async Task<List<Section>> GetAllSectionsAsync(bool onlyPublished = true)
    {
        var query = _db.Sections.Include(s => s.Pages).AsQueryable();
        if (onlyPublished)
            query = query.Where(s => s.IsPublished);
        return await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<Section?> GetSectionBySlugAsync(string slug)
    {
        return await _db.Sections
            .Include(s => s.Pages)
            .FirstOrDefaultAsync(s => s.Slug == slug);
    }

    public async Task<Section?> GetSectionByIdAsync(int id)
    {
        return await _db.Sections
            .Include(s => s.Pages)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Section> CreateSectionWithAutoTranslationAsync(CreateSectionViewModel model)
    {
        if (await _db.Sections.AnyAsync(s => s.Slug == model.Slug))
            throw new InvalidOperationException($"Slug '{model.Slug}' already exists.");

        var section = new Section
        {
            Slug = model.Slug,
            FlagImage = model.FlagImage,
            StatusColor = model.StatusColor,
            CreatedAt = DateTime.UtcNow,
            IsPublished = true
        };
        _db.Sections.Add(section);
        await _db.SaveChangesAsync();

        var allLanguages = await _db.Languages.ToListAsync();
        var baseLang = model.BaseLanguageCode;

        // Создаём страницу на базовом языке
        var basePage = new Page
        {
            SystemName = "case",
            Name = model.BaseTitle,
            LanguageCode = baseLang,
            SectionId = section.Id,
            Status = model.BaseStatus,
            Summary = model.BaseSummary
        };
        _db.Pages.Add(basePage);
        await _db.SaveChangesAsync();

        // Генерируем шаблонные блоки для базовой страницы
        var baseBlocks = _templateBuilder.BuildTemplateBlocks(basePage.Id, model.Slug, baseLang, model).ToList();
        foreach (var block in baseBlocks)
        {
            var style = await _blockService.GetStyleByNameAsync(block.StyleName);
            if (style == null) continue;
            block.PageId = basePage.Id;
            block.StyleId = style.Id;
            _db.ContentBlocks.Add(block);
        }
        await _db.SaveChangesAsync();

        // Для каждого другого языка создаём переведённую страницу
        foreach (var lang in allLanguages.Where(l => l.Code != baseLang))
        {
            var translatedName = await _translationService.TranslateAsync(model.BaseTitle, baseLang, lang.Code);
            var translatedStatus = string.IsNullOrEmpty(model.BaseStatus) ? "" : await _translationService.TranslateAsync(model.BaseStatus, baseLang, lang.Code);
            var translatedSummary = string.IsNullOrEmpty(model.BaseSummary) ? "" : await _translationService.TranslateAsync(model.BaseSummary, baseLang, lang.Code);

            var page = new Page
            {
                SystemName = "case",
                Name = translatedName,
                LanguageCode = lang.Code,
                SectionId = section.Id,
                Status = translatedStatus,
                Summary = translatedSummary
            };
            _db.Pages.Add(page);
            await _db.SaveChangesAsync();

            // Копируем и переводим блоки
            foreach (var block in baseBlocks)
            {
                var translatedContent = await _translationService.TranslateAsync(block.Content, baseLang, lang.Code);
                _db.ContentBlocks.Add(new ContentBlock
                {
                    PageId = page.Id,
                    Order = block.Order,
                    StyleId = block.StyleId,
                    Content = translatedContent,
                    ExtraAttribute = block.ExtraAttribute,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            await _db.SaveChangesAsync();
        }

        return section;
    }

    public async Task UpdateSectionAsync(Section section)
    {
        _db.Sections.Update(section);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteSectionAsync(int id)
    {
        var section = await _db.Sections.FindAsync(id);
        if (section != null)
        {
            _db.Sections.Remove(section);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<Page?> GetSectionPageAsync(string slug, string lang)
    {
        return await _db.Pages
            .Include(p => p.Section)
            .FirstOrDefaultAsync(p => p.Section != null && p.Section.Slug == slug && p.LanguageCode == lang);
    }

    public async Task AddLanguageToAllSectionsAsync(string languageCode)
    {
        var sections = await _db.Sections.ToListAsync();

        foreach (var section in sections)
        {
            if (await _db.Pages.AnyAsync(p => p.SectionId == section.Id && p.LanguageCode == languageCode))
                continue;

            var sourcePage = await _db.Pages
                .Include(p => p.ContentBlocks)
                .FirstOrDefaultAsync(p => p.SectionId == section.Id && p.LanguageCode != languageCode);

            string translatedName, translatedStatus, translatedSummary;
            string sourceLanguage = sourcePage?.LanguageCode ?? "en";

            if (sourcePage != null)
            {
                translatedName = await _translationService.TranslateAsync(sourcePage.Name, sourceLanguage, languageCode);
                translatedStatus = await _translationService.TranslateAsync(sourcePage.Status ?? "", sourceLanguage, languageCode);
                translatedSummary = await _translationService.TranslateAsync(sourcePage.Summary ?? "", sourceLanguage, languageCode);
            }
            else
            {
                translatedName = section.Slug;
                translatedStatus = "";
                translatedSummary = "";
            }

            var newPage = new Page
            {
                SystemName = "case",
                Name = translatedName,
                LanguageCode = languageCode,
                SectionId = section.Id,
                Status = translatedStatus,
                Summary = translatedSummary
            };
            _db.Pages.Add(newPage);
            await _db.SaveChangesAsync();

            foreach (var block in sourcePage.ContentBlocks.OrderBy(b => b.Order))
            {
                string? extraAttr = block.ExtraAttribute;
                string content = block.Content ?? "";

                // Нормализация e-dots: если extraAttr пуст, берём первый цвет из content
                if (block.Style.Name == "e-dots")
                {
                    if (string.IsNullOrEmpty(extraAttr) && !string.IsNullOrEmpty(content))
                    {
                        var colors = content.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        if (colors.Length > 0) extraAttr = colors[0].Trim();
                    }
                    content = null; // контент для e-dots больше не используется
                }

                var translatedContent = (block.Style.Name != "e-dots")
                    ? await _translationService.TranslateAsync(content ?? "", sourceLanguage, languageCode)
                    : null;

                _db.ContentBlocks.Add(new ContentBlock
                {
                    PageId = newPage.Id,
                    Order = block.Order,
                    StyleId = block.StyleId,
                    Content = translatedContent,
                    ExtraAttribute = extraAttr,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
    }

    public async Task CopyAndTranslateSectionBlocksAsync(string slug, string sourceLang, string targetLang)
    {
        var section = await _db.Sections.FirstOrDefaultAsync(s => s.Slug == slug)
            ?? throw new InvalidOperationException("Раздел не найден");
        var sourcePage = await _blockService.GetPageAsync("case", sourceLang, section.Id)
            ?? throw new InvalidOperationException("Исходная страница не найдена");
        var targetPage = await _blockService.GetPageAsync("case", targetLang, section.Id)
            ?? throw new InvalidOperationException("Целевая страница не найдена");

        var oldBlocks = _db.ContentBlocks.Where(b => b.PageId == targetPage.Id);
        _db.ContentBlocks.RemoveRange(oldBlocks);

        var sourceBlocks = await _db.ContentBlocks.Where(b => b.PageId == sourcePage.Id).ToListAsync();
        foreach (var block in sourceBlocks.OrderBy(b => b.Order))
        {
            var translatedContent = await _translationService.TranslateAsync(block.Content, sourceLang, targetLang);
            _db.ContentBlocks.Add(new ContentBlock
            {
                PageId = targetPage.Id,
                Order = block.Order,
                StyleId = block.StyleId,
                Content = translatedContent,
                ExtraAttribute = block.ExtraAttribute,
                UpdatedAt = DateTime.UtcNow
            });
        }
        await _db.SaveChangesAsync();
    }
}