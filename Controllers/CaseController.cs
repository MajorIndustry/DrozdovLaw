using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;
using Microsoft.AspNetCore.Mvc;

namespace DrozdovLaw.Controllers;

public class CaseController : Controller
{
    private readonly ICaseService _caseService;
    private readonly IBlockService _blockService;

    public CaseController(ICaseService caseService, IBlockService blockService)
    {
        _caseService = caseService;
        _blockService = blockService;
    }

    public async Task<IActionResult> List(string lang = "ru")
    {
        var cases = await _caseService.GetAllCasesAsync();
        return View(new CasesListViewModel { Language = lang, Cases = cases });
    }

    public async Task<IActionResult> Detail(string slug, string lang = "ru")
    {
        var meta = await _caseService.GetCaseBySlugAsync(slug);
        if (meta == null) return NotFound();

        var pageName = $"case-{slug}-{lang}";
        var blocks = await _blockService.GetPageBlocksAsync(pageName);
        var allCases = await _caseService.GetAllCasesAsync();
        var similar = allCases
            .Where(c => c.IsPublished && c.Slug != slug)
            .Take(4)
            .ToList();

        ViewBag.Lang = lang;
        ViewBag.PageLayoutClass = "m-light";
        ViewBag.Title = lang == "ru" ? meta.TitleRu : meta.TitleEn;

        return View(new CaseViewModel
        {
            PageName = pageName,
            Language = lang,
            Blocks = blocks,
            Meta = meta,
            SimilarCases = similar
        });
    }
}