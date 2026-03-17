using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;
using Microsoft.AspNetCore.Mvc;

namespace DrozdovLaw.Controllers;

public class WhoWeAreController : Controller
{
    private readonly IBlockService _blockService;
    private readonly ICaseService _caseService;

    public WhoWeAreController(IBlockService blockService, ICaseService caseService)
    {
        _blockService = blockService;
        _caseService = caseService;
    }

    public async Task<IActionResult> Index(string lang = "ru")
    {
        var pageName = lang == "en" ? "whoweare-en" : "whoweare-ru";
        var blocks = await _blockService.GetPageBlocksAsync(pageName);
        var cases = await _caseService.GetAllCasesAsync();
        ViewBag.Lang = lang;
        return View(new PageViewModel
        {
            PageName = pageName,
            Language = lang,
            Blocks = blocks,
            Cases = cases.Where(c => c.IsPublished).ToList()
        });
    }
}