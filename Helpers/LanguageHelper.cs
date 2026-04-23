using DrozdovLaw.Data;
using Microsoft.EntityFrameworkCore;

namespace DrozdovLaw.Helpers
{
    public static class LanguageHelper
    {
        public static async Task<List<(string Code, string Name)>> GetActiveLanguagesAsync(AppDbContext db)
        {
            // Языки считаются активными, если для них есть хотя бы одна опубликованная страница раздела (или статическая)
            var codes = await db.Pages
                .Where(p => p.Section == null || p.Section.IsPublished)
                .Select(p => p.LanguageCode)
                .Distinct()
                .ToListAsync();

            var languages = await db.Languages
                .Where(l => codes.Contains(l.Code))
                .OrderBy(l => l.Code)
                .Select(l => new { l.Code, l.Name })
                .ToListAsync();

            return languages.Select(l => (l.Code, l.Name)).ToList();
        }
    }
}
