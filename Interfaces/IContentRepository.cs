using DrozdovLaw.Models;

namespace DrozdovLaw.Interfaces
{
    public interface IContentRepository
    {
        Task<ContentData> LoadAsync();
        Task SaveAsync(ContentData data);
    }
}
