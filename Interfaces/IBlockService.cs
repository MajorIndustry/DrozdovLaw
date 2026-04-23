using DrozdovLaw.Models;

namespace DrozdovLaw.Interfaces;

public interface IBlockService
{
    // Блоки
    Task<List<ContentBlock>> GetPageBlocksAsync(string systemName, string lang, int? sectionId = null);
    Task<ContentBlock?> GetBlockByIdAsync(int id);
    Task<ContentBlock> CreateBlockAsync(string systemName, string lang, string displayName, string styleName, string content, string? extraAttribute = null, int? sectionId = null);
    Task UpdateBlockAsync(int id, string content, string? extraAttribute, int styleId);
    Task DeleteBlockAsync(int id);
    Task ReorderBlocksAsync(string systemName, string lang, List<int> orderedIds, int? sectionId = null);

    // Страницы
    Task<Page?> GetPageAsync(string systemName, string lang, int? sectionId = null);
    Task<int> GetOrCreatePageIdAsync(string systemName, string lang, string displayName, int? sectionId = null);
    Task UpdatePageAsync(Page page);

    // Стили
    Task<BlockStyle?> GetStyleByNameAsync(string name);
    Task<List<BlockStyle>> GetAllStylesAsync();

    // Языки
    Task<List<Language>> GetAllLanguagesAsync();
}