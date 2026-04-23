using DrozdovLaw.Data;
using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;
using Microsoft.EntityFrameworkCore;

namespace DrozdovLaw.Services;

public class BlockService : IBlockService
{
    private readonly AppDbContext _db;

    public BlockService(AppDbContext dbContext)
    {
        _db = dbContext;
    }

    #region Blocks

    public async Task<List<ContentBlock>> GetPageBlocksAsync(string systemName, string lang, int? sectionId = null)
    {
        var page = await GetPageAsync(systemName, lang, sectionId);
        if (page == null)
            return new List<ContentBlock>();

        return await _db.ContentBlocks
            .Where(b => b.PageId == page.Id)
            .Include(b => b.Style)
            .OrderBy(b => b.Order)
            .ToListAsync();
    }

    public async Task<ContentBlock?> GetBlockByIdAsync(int id)
    {
        return await _db.ContentBlocks
            .Include(b => b.Style)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<ContentBlock> CreateBlockAsync(string systemName, string lang, string displayName, string styleName, string content, string? extraAttribute = null, int? sectionId = null)
    {
        var pageId = await GetOrCreatePageIdAsync(systemName, lang, displayName, sectionId);
        var style = await GetStyleByNameAsync(styleName)
            ?? throw new InvalidOperationException($"Style '{styleName}' not found.");

        var maxOrder = await _db.ContentBlocks
            .Where(b => b.PageId == pageId)
            .MaxAsync(b => (int?)b.Order) ?? 0;

        var block = new ContentBlock
        {
            PageId = pageId,
            StyleId = style.Id,
            Content = content,
            ExtraAttribute = extraAttribute,
            Order = maxOrder + 1,
            UpdatedAt = DateTime.UtcNow
        };

        _db.ContentBlocks.Add(block);
        await _db.SaveChangesAsync();
        return block;
    }

    public async Task UpdateBlockAsync(int id, string content, string? extraAttribute, int styleId)
    {
        var block = await _db.ContentBlocks.FindAsync(id)
            ?? throw new InvalidOperationException($"Block with id {id} not found.");

        block.Content = content;
        block.ExtraAttribute = extraAttribute;
        block.StyleId = styleId;
        block.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteBlockAsync(int id)
    {
        var block = await _db.ContentBlocks.FindAsync(id);
        if (block != null)
        {
            _db.ContentBlocks.Remove(block);
            await _db.SaveChangesAsync();
        }
    }

    public async Task ReorderBlocksAsync(string systemName, string lang, List<int> orderedIds, int? sectionId = null)
    {
        var page = await GetPageAsync(systemName, lang, sectionId);
        if (page == null)
            return;

        var blocks = await _db.ContentBlocks
            .Where(b => b.PageId == page.Id && orderedIds.Contains(b.Id))
            .ToListAsync();

        for (int i = 0; i < orderedIds.Count; i++)
        {
            var block = blocks.FirstOrDefault(b => b.Id == orderedIds[i]);
            if (block != null)
                block.Order = i + 1;
        }

        await _db.SaveChangesAsync();
    }

    #endregion

    #region Pages

    public async Task<Page?> GetPageAsync(string systemName, string lang, int? sectionId = null)
    {
        return await _db.Pages
            .Include(p => p.Section)
            .FirstOrDefaultAsync(p => p.SystemName == systemName && p.LanguageCode == lang && p.SectionId == sectionId);
    }

    public async Task<int> GetOrCreatePageIdAsync(string systemName, string lang, string displayName, int? sectionId = null)
    {
        var page = await _db.Pages
            .FirstOrDefaultAsync(p => p.SystemName == systemName && p.LanguageCode == lang && p.SectionId == sectionId);
        if (page != null)
            return page.Id;

        page = new Page
        {
            SystemName = systemName,
            Name = displayName,
            LanguageCode = lang,
            SectionId = sectionId
        };
        _db.Pages.Add(page);
        await _db.SaveChangesAsync();
        return page.Id;
    }

    public async Task UpdatePageAsync(Page page)
    {
        _db.Pages.Update(page);
        await _db.SaveChangesAsync();
    }

    #endregion

    #region Styles

    public async Task<BlockStyle?> GetStyleByNameAsync(string name)
    {
        return await _db.BlockStyles.FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task<List<BlockStyle>> GetAllStylesAsync()
    {
        return await _db.BlockStyles.OrderBy(s => s.Name).ToListAsync();
    }

    #endregion

    #region Languages

    public async Task<List<Language>> GetAllLanguagesAsync()
    {
        return await _db.Languages.OrderBy(l => l.Code).ToListAsync();
    }

    #endregion
}