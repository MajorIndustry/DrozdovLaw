using DrozdovLaw.Models;

namespace DrozdovLaw.Interfaces;

public interface IBlockService
{
    Task<List<ContentBlock>> GetPageBlocksAsync(string pageName);
    Task<ContentBlock?> GetBlockAsync(string id);
    Task SaveBlockAsync(ContentBlock block);
    Task DeleteBlockAsync(string id);
    Task ReorderAsync(string pageName, List<string> orderedIds);
}