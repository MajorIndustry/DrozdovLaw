using DrozdovLaw.Data;
using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;
using Microsoft.EntityFrameworkCore;

namespace DrozdovLaw.Services;

public class LanguageService : ILanguageService
{
    private readonly AppDbContext _db;
    private readonly ISectionService _sectionService;
    private readonly ITranslationService _translationService;

    public LanguageService(AppDbContext db, ISectionService sectionService, ITranslationService translationService)
    {
        _db = db;
        _sectionService = sectionService;
        _translationService = translationService;
    }

    public async Task<List<Language>> GetAllAsync() =>
        await _db.Languages.OrderBy(l => l.Code).ToListAsync();

    public async Task<Language?> GetByCodeAsync(string code) =>
        await _db.Languages.FindAsync(code);

    public async Task CreateAsync(Language language)
    {
        _db.Languages.Add(language);
        await _db.SaveChangesAsync();

        // 1. Автоматически добавляем страницы для всех разделов
        await _sectionService.AddLanguageToAllSectionsAsync(language.Code);

        // 2. Автоматически создаём/переводим статические страницы (whoweare)
        await AddLanguageToStaticPagesAsync(language.Code);
    }

    public async Task UpdateAsync(Language language)
    {
        _db.Languages.Update(language);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string code)
    {
        var language = await _db.Languages
            .Include(l => l.Pages)
            .FirstOrDefaultAsync(l => l.Code == code);
        if (language != null)
        {
            _db.Languages.Remove(language); // каскад удалит связанные страницы
            await _db.SaveChangesAsync();
        }
    }

    private async Task AddLanguageToStaticPagesAsync(string languageCode)
    {
        var staticSystemNames = new[] { "whoweare" }; // при необходимости добавить другие

        foreach (var systemName in staticSystemNames)
        {
            // Проверяем, нет ли уже страницы с таким языком
            if (await _db.Pages.AnyAsync(p => p.SystemName == systemName && p.LanguageCode == languageCode))
                continue;

            // Находим любую существующую страницу этой статики на другом языке как источник для перевода
            var sourcePage = await _db.Pages
                .Include(p => p.ContentBlocks)
                .FirstOrDefaultAsync(p => p.SystemName == systemName && p.LanguageCode != languageCode);

            if (sourcePage != null)
            {
                // Переводим метаданные страницы (название, статус, описание)
                var translatedName = await _translationService.TranslateAsync(sourcePage.Name, sourcePage.LanguageCode, languageCode);
                string? translatedStatus = sourcePage.Status != null
                    ? await _translationService.TranslateAsync(sourcePage.Status, sourcePage.LanguageCode, languageCode)
                    : null;
                string? translatedSummary = sourcePage.Summary != null
                    ? await _translationService.TranslateAsync(sourcePage.Summary, sourcePage.LanguageCode, languageCode)
                    : null;

                var newPage = new Page
                {
                    SystemName = systemName,
                    Name = translatedName,
                    LanguageCode = languageCode,
                    SectionId = null,
                    Status = translatedStatus,
                    Summary = translatedSummary
                };
                _db.Pages.Add(newPage);
                await _db.SaveChangesAsync();

                // Копируем и переводим блоки
                foreach (var block in sourcePage.ContentBlocks.OrderBy(b => b.Order))
                {
                    var translatedContent = await _translationService.TranslateAsync(block.Content, sourcePage.LanguageCode, languageCode);
                    _db.ContentBlocks.Add(new ContentBlock
                    {
                        PageId = newPage.Id,
                        Order = block.Order,
                        StyleId = block.StyleId,
                        Content = translatedContent,
                        ExtraAttribute = block.ExtraAttribute,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                await _db.SaveChangesAsync();
            }
            else
            {
                // Если вообще нет ни одной версии (редкий случай) — создаём пустую заглушку
                var newPage = new Page
                {
                    SystemName = systemName,
                    Name = systemName, // потом можно отредактировать
                    LanguageCode = languageCode,
                    SectionId = null
                };
                _db.Pages.Add(newPage);
                await _db.SaveChangesAsync();
            }
        }
    }
}