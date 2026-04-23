using DrozdovLaw.Interfaces;
using DrozdovLaw.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class CaseController : Controller
{
    private readonly ISectionService _sectionService;
    private readonly IBlockService _blockService;
    private readonly ILanguageService _languageService;

    public CaseController(ISectionService sectionService, IBlockService blockService, ILanguageService languageService)
    {
        _sectionService = sectionService;
        _blockService = blockService;
        _languageService = languageService;
    }

    public async Task<IActionResult> List(string lang = "ru")
    {
        var sections = await _sectionService.GetAllSectionsAsync();
        ViewBag.AvailableLanguages = await _languageService.GetAllAsync();
        return View(new CasesListViewModel { Language = lang, Sections = sections });
    }

    public async Task<IActionResult> Detail(string slug, string lang = "ru")
    {
        var section = await _sectionService.GetSectionBySlugAsync(slug);
        if (section == null) return NotFound();

        var page = await _sectionService.GetSectionPageAsync(slug, lang);
        if (page == null) return NotFound();

        var blocks = await _blockService.GetPageBlocksAsync("case", lang, section.Id);
        var allSections = await _sectionService.GetAllSectionsAsync();
        var similar = allSections.Where(s => s.IsPublished && s.Slug != slug).Take(4).ToList();

        ViewBag.Lang = lang;
        ViewBag.PageLayoutClass = "m-light";
        ViewBag.Title = page.Status ?? section.Slug;
        ViewBag.AvailableLanguages = await _languageService.GetAllAsync();

        return View(new CaseViewModel
        {
            Page = page,
            Language = lang,
            Blocks = blocks,
            Section = section,
            SimilarSections = similar
        });
    }
}