using DrozdovLaw.Models;
using DrozdovLaw.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrozdovLaw.Controllers;

public class CaseController : Controller
{
    private readonly ContentService _content;

    public CaseController(ContentService content) => _content = content;

    // GET /Cases?lang=ru  — список всех кейсов
    public async Task<IActionResult> List(string lang = "ru")
    {
        var cases = await _content.GetAllCasesAsync();
        return View(new CasesListViewModel { Language = lang, Cases = cases });
    }

    // GET /Cases/{slug}?lang=ru  — конкретный кейс
    public async Task<IActionResult> Detail(string slug, string lang = "ru")
    {
        var meta = await _content.GetCaseBySlugAsync(slug);
        if (meta == null) return NotFound();

        var pageName = $"case-{slug}-{lang}";
        var blocks = await _content.GetPageBlocksAsync(pageName);

        ViewBag.Lang = lang;
        ViewBag.PageLayoutClass = "m-light";
        ViewBag.Title = lang == "ru" ? meta.TitleRu : meta.TitleEn;

        return View(new CaseViewModel
        {
            PageName = pageName,
            Language = lang,
            Blocks = blocks,
            Meta = meta
        });
    }
}
