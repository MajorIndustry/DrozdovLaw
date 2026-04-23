using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;
using DrozdovLaw.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class WhoWeAreController : Controller
{
    private readonly IBlockService _blockService;
    private readonly ISectionService _sectionService;
    private readonly ILanguageService _languageService;

    public WhoWeAreController(IBlockService blockService, ISectionService sectionService, ILanguageService languageService)
    {
        _blockService = blockService;
        _sectionService = sectionService;
        _languageService = languageService;
    }

    public async Task<IActionResult> Index(string lang = "ru")
    {
        var pageId = await _blockService.GetOrCreatePageIdAsync("whoweare", lang,
            lang == "ru" ? " то мы" : "Who we are", null);
        var page = await _blockService.GetPageAsync("whoweare", lang, null);
        var blocks = await _blockService.GetPageBlocksAsync("whoweare", lang, null);
        var sections = await _sectionService.GetAllSectionsAsync();

        ViewBag.Lang = lang;
        ViewBag.AvailableLanguages = await _languageService.GetAllAsync();
        return View(new PageViewModel
        {
            Page = page ?? new Page { SystemName = "whoweare", Name = lang == "ru" ? " то мы" : "Who we are", LanguageCode = lang },
            Language = lang,
            Blocks = blocks,
            Sections = sections
        });
    }
}