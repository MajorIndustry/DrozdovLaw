using System.Text.Json;
using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;

namespace DrozdovLaw.Repositories;

public class JsonContentRepository : IContentRepository
{
    private readonly string _dataPath;
    private readonly ILogger<JsonContentRepository> _logger;
    private static readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonContentRepository(IWebHostEnvironment env, ILogger<JsonContentRepository> logger)
    {
        _logger = logger;
        _dataPath = Path.Combine(env.ContentRootPath, "Data", "content.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_dataPath)!);
    }

    public async Task<ContentData> LoadAsync()
    {
        if (!File.Exists(_dataPath)) return new ContentData();
        try
        {
            var json = await File.ReadAllTextAsync(_dataPath);
            return JsonSerializer.Deserialize<ContentData>(json, _jsonOptions) ?? new ContentData();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка чтения content.json");
            return new ContentData();
        }
    }

    public async Task SaveAsync(ContentData data)
    {
        await _lock.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            await File.WriteAllTextAsync(_dataPath, json);
        }
        finally
        {
            _lock.Release();
        }
    }
}