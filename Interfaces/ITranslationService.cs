using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrozdovLaw.Interfaces;

public interface ITranslationService
{
    Task<string> TranslateAsync(string text, string sourceLanguage, string targetLanguage);
    Task<string> TranslateAsync(string text, string targetLanguage);
    Task<List<LanguageInfo>> GetSupportedLanguagesAsync();
}

public class LanguageInfo
{
    public string Code { get; set; }
    public string Name { get; set; }
}