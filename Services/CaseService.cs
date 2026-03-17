using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;

namespace DrozdovLaw.Services;

public class CaseService : ICaseService
{
    private readonly IContentRepository _repository;
    private readonly ICaseTemplateBuilder _templateBuilder;

    public CaseService(IContentRepository repository, ICaseTemplateBuilder templateBuilder)
    {
        _repository = repository;
        _templateBuilder = templateBuilder;
    }

    public async Task<List<CaseMeta>> GetAllCasesAsync()
    {
        var data = await _repository.LoadAsync();
        return data.Cases.OrderByDescending(c => c.CreatedAt).ToList();
    }

    public async Task<CaseMeta?> GetCaseBySlugAsync(string slug)
    {
        var data = await _repository.LoadAsync();
        return data.Cases.FirstOrDefault(c => c.Slug == slug);
    }

    public async Task<CaseMeta?> GetCaseByIdAsync(string id)
    {
        var data = await _repository.LoadAsync();
        return data.Cases.FirstOrDefault(c => c.Id == id);
    }

    public async Task<CaseMeta> CreateCaseAsync(CreateCaseViewModel vm)
    {
        var data = await _repository.LoadAsync();
        var slug = vm.Slug.ToLower().Trim().Replace(" ", "-");

        var meta = new CaseMeta
        {
            Slug = slug,
            TitleRu = vm.TitleRu,
            TitleEn = vm.TitleEn,
            StatusColor = vm.StatusColor,
            StatusRu = vm.StatusRu,
            StatusEn = vm.StatusEn,
            LocationRu = vm.LocationRu,
            LocationEn = vm.LocationEn,
            SummaryRu = vm.SummaryRu,
            SummaryEn = vm.SummaryEn,
            FlagImage = vm.FlagImage,
            CreatedAt = DateTime.UtcNow,
            IsPublished = true
        };
        data.Cases.Add(meta);

        var ruBlocks = _templateBuilder.BuildTemplateBlocks($"case-{slug}-ru", slug, "ru", vm);
        var enBlocks = _templateBuilder.BuildTemplateBlocks($"case-{slug}-en", slug, "en", vm);
        data.Blocks.AddRange(ruBlocks);
        data.Blocks.AddRange(enBlocks);

        await _repository.SaveAsync(data);
        return meta;
    }

    public async Task UpdateCaseMetaAsync(CaseMeta updated)
    {
        var data = await _repository.LoadAsync();
        var existing = data.Cases.FirstOrDefault(c => c.Id == updated.Id);
        if (existing == null) return;

        existing.TitleRu = updated.TitleRu;
        existing.TitleEn = updated.TitleEn;
        existing.StatusColor = updated.StatusColor;
        existing.StatusRu = updated.StatusRu;
        existing.StatusEn = updated.StatusEn;
        existing.LocationRu = updated.LocationRu;
        existing.LocationEn = updated.LocationEn;
        existing.SummaryRu = updated.SummaryRu;
        existing.SummaryEn = updated.SummaryEn;
        existing.FlagImage = updated.FlagImage;
        existing.IsPublished = updated.IsPublished;

        await _repository.SaveAsync(data);
    }

    public async Task DeleteCaseAsync(string id)
    {
        var data = await _repository.LoadAsync();
        var meta = data.Cases.FirstOrDefault(c => c.Id == id);
        if (meta == null) return;

        data.Blocks.RemoveAll(b => b.PageName == $"case-{meta.Slug}-ru"
                                || b.PageName == $"case-{meta.Slug}-en");
        data.Cases.Remove(meta);

        await _repository.SaveAsync(data);
    }
}