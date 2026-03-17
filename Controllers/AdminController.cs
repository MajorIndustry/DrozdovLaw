using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;
using Microsoft.AspNetCore.Mvc;

namespace DrozdovLaw.Controllers;

public class AdminController : Controller
{
    private readonly IBlockService _blockService;
    private readonly ICaseService _caseService;
    private readonly IPreviewService _previewService;

    public AdminController(IBlockService blockService, ICaseService caseService, IPreviewService previewService)
    {
        _blockService = blockService;
        _caseService = caseService;
        _previewService = previewService;
    }

    public async Task<IActionResult> Index(string section = "cases", string? page = null,
                                           string? slug = null, string lang = "ru")
    {
        var vm = new AdminIndexViewModel { ActiveLang = lang, ActiveSection = section };

        switch (section)
        {
            case "cases":
                vm.Cases = await _caseService.GetAllCasesAsync();
                break;

            case "case-detail" when slug != null:
                vm.SelectedCase = await _caseService.GetCaseBySlugAsync(slug);
                page ??= $"case-{slug}-{lang}";
                vm.SelectedPage = page;
                vm.Blocks = await _blockService.GetPageBlocksAsync(page);
                vm.Cases = await _caseService.GetAllCasesAsync();
                break;

            case "whoweare":
                page ??= $"whoweare-{lang}";
                vm.SelectedPage = page;
                vm.Blocks = await _blockService.GetPageBlocksAsync(page);
                break;
        }

        return View(vm);
    }

    public async Task<IActionResult> Edit(string id, string returnPage, string lang = "ru",
                                          string section = "cases", string? slug = null)
    {
        var block = await _blockService.GetBlockAsync(id);
        if (block == null) return NotFound();
        ViewBag.Section = section; ViewBag.Slug = slug;
        return View(new AdminEditViewModel { Block = block, ReturnPage = returnPage, Lang = lang });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AdminEditViewModel vm, string section = "cases", string? slug = null)
    {
        if (!ModelState.IsValid) return View(vm);
        await _blockService.SaveBlockAsync(vm.Block);
        TempData["Success"] = "Блок сохранён";
        TempData["ShowPreview"] = "true";
        return RedirectToAction(nameof(Index), new { section, slug, page = vm.ReturnPage, lang = vm.Lang });
    }

    public IActionResult Create(string page, string lang = "ru", string section = "cases", string? slug = null)
    {
        var block = new ContentBlock { PageName = page };
        ViewBag.Section = section; ViewBag.Slug = slug;
        return View("Edit", new AdminEditViewModel { Block = block, ReturnPage = page, Lang = lang });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id, string returnPage, string lang = "ru",
                                            string section = "cases", string? slug = null)
    {
        await _blockService.DeleteBlockAsync(id);
        TempData["Success"] = "Блок удалён";
        return RedirectToAction(nameof(Index), new { section, slug, page = returnPage, lang });
    }

    [HttpPost]
    public async Task<IActionResult> Reorder([FromBody] ReorderRequest req)
    {
        await _blockService.ReorderAsync(req.Page, req.Ids);
        return Ok();
    }

    public IActionResult CreateCase() => View(new CreateCaseViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCase(CreateCaseViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var meta = await _caseService.CreateCaseAsync(vm);
        TempData["Success"] = $"Кейс «{meta.TitleRu}» создан";
        return RedirectToAction(nameof(Index), new { section = "case-detail", slug = meta.Slug, lang = "ru" });
    }

    public async Task<IActionResult> EditCaseMeta(string id, string lang = "ru")
    {
        var c = await _caseService.GetCaseByIdAsync(id);
        if (c == null) return NotFound();
        return View(new EditCaseMetaViewModel { Case = c, Lang = lang });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCaseMeta(EditCaseMetaViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        await _caseService.UpdateCaseMetaAsync(vm.Case);
        TempData["Success"] = "Метаданные кейса обновлены";
        return RedirectToAction(nameof(Index), new { section = "case-detail", slug = vm.Case.Slug, lang = vm.Lang });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCase(string id)
    {
        var c = await _caseService.GetCaseByIdAsync(id);
        await _caseService.DeleteCaseAsync(id);
        TempData["Success"] = "Кейс удалён";
        return RedirectToAction(nameof(Index), new { section = "cases" });
    }

    public async Task<IActionResult> Preview(string page)
    {
        var (viewName, viewModel) = await _previewService.GetPreviewAsync(page);
        var lang = page.EndsWith("-en") ? "en" : "ru";
        ViewBag.IsPreview = true;
        ViewBag.PreviewPage = page;
        ViewBag.Lang = lang;
        ViewBag.PageLayoutClass = page.Contains("case") ? "m-light" : "";
        return View(viewName, viewModel);
    }
}

public class ReorderRequest
{
    public string Page { get; set; } = string.Empty;
    public List<string> Ids { get; set; } = new();
}