using DrozdovLaw.Models;
using DrozdovLaw.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrozdovLaw.Controllers;

public class WhoWeAreController : Controller
{
    private readonly ContentService _content;
    public WhoWeAreController(ContentService content) => _content = content;

    public async Task<IActionResult> Index(string lang = "ru")
    {
        var pageName = lang == "en" ? "whoweare-en" : "whoweare-ru";
        var blocks = await _content.GetPageBlocksAsync(pageName);
        ViewBag.Lang = lang;
        return View(new PageViewModel { PageName = pageName, Language = lang, Blocks = blocks });
    }
}
