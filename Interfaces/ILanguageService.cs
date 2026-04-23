using DrozdovLaw.Models;

namespace DrozdovLaw.Interfaces;

public interface ILanguageService
{
    Task<List<Language>> GetAllAsync();
    Task<Language?> GetByCodeAsync(string code);
    Task CreateAsync(Language language);
    Task UpdateAsync(Language language);
    Task DeleteAsync(string code);
}