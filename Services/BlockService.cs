using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;

namespace DrozdovLaw.Services;

public class BlockService : IBlockService
{
    private readonly IContentRepository _repository;

    public BlockService(IContentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ContentBlock>> GetPageBlocksAsync(string pageName)
    {
        var data = await _repository.LoadAsync();
        return data.Blocks.Where(b => b.PageName == pageName).OrderBy(b => b.Order).ToList();
    }

    public async Task<ContentBlock?> GetBlockAsync(string id)
    {
        var data = await _repository.LoadAsync();
        return data.Blocks.FirstOrDefault(b => b.Id == id);
    }

    public async Task SaveBlockAsync(ContentBlock block)
    {
        var data = await _repository.LoadAsync();
        var existing = data.Blocks.FirstOrDefault(b => b.Id == block.Id);
        if (existing != null)
        {
            existing.Content = block.Content;
            existing.Style = block.Style;
            existing.Order = block.Order;
            existing.ExtraAttribute = block.ExtraAttribute;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            block.UpdatedAt = DateTime.UtcNow;
            data.Blocks.Add(block);
        }
        await _repository.SaveAsync(data);
    }

    public async Task DeleteBlockAsync(string id)
    {
        var data = await _repository.LoadAsync();
        data.Blocks.RemoveAll(b => b.Id == id);
        await _repository.SaveAsync(data);
    }

    public async Task ReorderAsync(string pageName, List<string> orderedIds)
    {
        var data = await _repository.LoadAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var b = data.Blocks.FirstOrDefault(x => x.Id == orderedIds[i] && x.PageName == pageName);
            if (b != null) b.Order = i + 1;
        }
        await _repository.SaveAsync(data);
    }
}