using DrozdovLaw.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace DrozdovLaw.Services;

public class TranslationService : ITranslationService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<TranslationService> _logger;

    public TranslationService(HttpClient httpClient, IConfiguration configuration, ILogger<TranslationService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["GoogleTranslate:ApiKey"]
            ?? throw new InvalidOperationException("GoogleTranslate:ApiKey is missing");
        _logger = logger;
    }

    public async Task<string> TranslateAsync(string text, string targetLanguage)
    {
        return await TranslateAsync(text, null, targetLanguage);
    }

    public async Task<string> TranslateAsync(string text, string sourceLanguage, string targetLanguage)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        try
        {
            var url = $"https://translation.googleapis.com/language/translate/v2?key={_apiKey}";

            // Строим тело запроса динамически
            var body = new Dictionary<string, object>
            {
                { "q", text },
                { "target", targetLanguage },
                { "format", "text" }
            };

            if (!string.IsNullOrEmpty(sourceLanguage))
            {
                body["source"] = sourceLanguage;
            }

            var response = await _httpClient.PostAsJsonAsync(url, body);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GoogleTranslateResponse>();
            return result?.Data?.Translations?.FirstOrDefault()?.TranslatedText ?? text;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Translate failed: {Text} -> {Target}", text, targetLanguage);
            return text; // fallback – оригинал
        }
    }
    public async Task<List<LanguageInfo>> GetSupportedLanguagesAsync()
    {
        try
        {
            var url = $"https://translation.googleapis.com/language/translate/v2/languages?key={_apiKey}&target=en";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GoogleLanguagesResponse>();
            return result?.Data?.Languages
                .Select(l => new LanguageInfo { Code = l.Language, Name = l.Name })
                .OrderBy(l => l.Name)
                .ToList() ?? new List<LanguageInfo>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load languages from Google API");
            // Fallback – небольшой статический список
            return new List<LanguageInfo>
        {
            new LanguageInfo { Code = "ru", Name = "Russian" },
            new LanguageInfo { Code = "en", Name = "English" },
            new LanguageInfo { Code = "de", Name = "German" },
            new LanguageInfo { Code = "fr", Name = "French" }
        };
        }
    }

    private class GoogleLanguagesResponse
    {
        public LanguagesData Data { get; set; }
        public class LanguagesData
        {
            public List<LanguageEntry> Languages { get; set; }
        }
        public class LanguageEntry
        {
            public string Language { get; set; }
            public string Name { get; set; }
        }
    }
    private class GoogleTranslateResponse
    {
        public TranslationData Data { get; set; }
    }

    private class TranslationData
    {
        public List<Translation> Translations { get; set; }
    }

    private class Translation
    {
        public string TranslatedText { get; set; }
    }
}