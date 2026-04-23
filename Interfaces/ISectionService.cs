using DrozdovLaw.Models;
using DrozdovLaw.Models.ViewModels;

namespace DrozdovLaw.Interfaces;

public interface ISectionService
{
    Task<List<Section>> GetAllSectionsAsync(bool onlyPublished = true);
    Task<Section?> GetSectionBySlugAsync(string slug);
    Task<Section?> GetSectionByIdAsync(int id);
    Task UpdateSectionAsync(Section section);
    Task DeleteSectionAsync(int id);
    Task<Page?> GetSectionPageAsync(string slug, string lang);
    Task AddLanguageToAllSectionsAsync(string languageCode);
    Task CopyAndTranslateSectionBlocksAsync(string slug, string sourceLang, string targetLang);
    Task<Section> CreateSectionWithAutoTranslationAsync(CreateSectionViewModel model);
}