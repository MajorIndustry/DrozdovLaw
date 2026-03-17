using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;

namespace DrozdovLaw.Services;

public class PreviewService : IPreviewService
{
    private readonly IBlockService _blockService;
    private readonly ICaseService _caseService;

    public PreviewService(IBlockService blockService, ICaseService caseService)
    {
        _blockService = blockService;
        _caseService = caseService;
    }

    public async Task<(string viewName, object viewModel)> GetPreviewAsync(string page)
    {
        var lang = page.EndsWith("-en") ? "en" : "ru";
        var blocks = await _blockService.GetPageBlocksAsync(page);

        if (page.StartsWith("case-") && !page.StartsWith("case-ru") && !page.StartsWith("case-en")
            || (page.StartsWith("case-") && page.Count(c => c == '-') >= 2))
        {
            var parts = page.Split('-');
            var slug = string.Join("-", parts[1..^1]);
            var meta = await _caseService.GetCaseBySlugAsync(slug);
            var viewName = "~/Views/Case/Detail.cshtml";
            var viewModel = new CaseViewModel
            {
                PageName = page,
                Language = lang,
                Blocks = blocks,
                Meta = meta
            };
            return (viewName, viewModel);
        }
        else
        {
            var viewName = "~/Views/WhoWeAre/Index.cshtml";
            var viewModel = new PageViewModel
            {
                PageName = page,
                Language = lang,
                Blocks = blocks
            };
            return (viewName, viewModel);
        }
    }
}