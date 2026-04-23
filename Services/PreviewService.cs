using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;
using DrozdovLaw.Models.ViewModels;

namespace DrozdovLaw.Services;

public class PreviewService : IPreviewService
{
    private readonly IBlockService _blockService;
    private readonly ISectionService _sectionService;

    public PreviewService(IBlockService blockService, ISectionService sectionService)
    {
        _blockService = blockService;
        _sectionService = sectionService;
    }

    public async Task<(string viewName, object viewModel)> GetPreviewAsync(string systemName, string lang, int? sectionId = null)
    {
        var page = await _blockService.GetPageAsync(systemName, lang, sectionId);
        var blocks = await _blockService.GetPageBlocksAsync(systemName, lang, sectionId);

        if (systemName == "case" && page?.Section != null)
        {
            var section = page.Section;
            var similarSections = (await _sectionService.GetAllSectionsAsync())
                                    .Where(s => s.IsPublished && s.Slug != section.Slug)
                                    .Take(4)
                                    .ToList();

            var viewModel = new CaseViewModel
            {
                Page = page,
                Language = lang,
                Blocks = blocks,
                Section = section,
                SimilarSections = similarSections
            };
            return ("~/Views/Case/Detail.cshtml", viewModel);
        }
        else
        {
            var sections = await _sectionService.GetAllSectionsAsync();
            var viewModel = new PageViewModel
            {
                Page = page ?? new Page { SystemName = systemName, Name = systemName, LanguageCode = lang },
                Language = lang,
                Blocks = blocks,
                Sections = sections
            };
            return ("~/Views/WhoWeAre/Index.cshtml", viewModel);
        }
    }
}