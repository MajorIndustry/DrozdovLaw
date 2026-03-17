using DrozdovLaw.Models;

namespace DrozdovLaw.Interfaces;

public interface ICaseService
{
    Task<List<CaseMeta>> GetAllCasesAsync();
    Task<CaseMeta?> GetCaseBySlugAsync(string slug);
    Task<CaseMeta?> GetCaseByIdAsync(string id);
    Task<CaseMeta> CreateCaseAsync(CreateCaseViewModel vm);
    Task UpdateCaseMetaAsync(CaseMeta updated);
    Task DeleteCaseAsync(string id);
}