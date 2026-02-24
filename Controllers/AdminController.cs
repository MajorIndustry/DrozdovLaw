using DrozdovLaw.Models;
using DrozdovLaw.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrozdovLaw.Controllers;

public class AdminController : Controller
{
    private readonly ContentService _content;
    public AdminController(ContentService content) => _content = content;

    // ── ГЛАВНАЯ СТРАНИЦА АДМИНКИ ──
    // GET /Admin?section=cases&lang=ru
    // GET /Admin?section=whoweare&page=whoweare-ru&lang=ru
    // GET /Admin?section=case-detail&slug=kk-vs-switzerland&page=case-kk-vs-switzerland-ru&lang=ru
    public async Task<IActionResult> Index(string section = "cases", string? page = null,
                                           string? slug = null, string lang = "ru")
    {
        var vm = new AdminIndexViewModel { ActiveLang = lang, ActiveSection = section };

        switch (section)
        {
            case "cases":
                vm.Cases = await _content.GetAllCasesAsync();
                break;

            case "case-detail" when slug != null:
                vm.SelectedCase = await _content.GetCaseBySlugAsync(slug);
                page ??= $"case-{slug}-{lang}";
                vm.SelectedPage = page;
                vm.Blocks = await _content.GetPageBlocksAsync(page);
                vm.Cases = await _content.GetAllCasesAsync();
                break;

            case "whoweare":
                page ??= $"whoweare-{lang}";
                vm.SelectedPage = page;
                vm.Blocks = await _content.GetPageBlocksAsync(page);
                break;
        }

        return View(vm);
    }

    // ── БЛОКИ ──

    public async Task<IActionResult> Edit(string id, string returnPage, string lang = "ru",
                                          string section = "cases", string? slug = null)
    {
        var block = await _content.GetBlockAsync(id);
        if (block == null) return NotFound();
        ViewBag.Section = section; ViewBag.Slug = slug;
        return View(new AdminEditViewModel { Block = block, ReturnPage = returnPage, Lang = lang });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AdminEditViewModel vm, string section = "cases", string? slug = null)
    {
        if (!ModelState.IsValid) return View(vm);
        await _content.SaveBlockAsync(vm.Block);
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
        await _content.DeleteBlockAsync(id);
        TempData["Success"] = "Блок удалён";
        return RedirectToAction(nameof(Index), new { section, slug, page = returnPage, lang });
    }

    [HttpPost]
    public async Task<IActionResult> Reorder([FromBody] ReorderRequest req)
    {
        await _content.ReorderAsync(req.Page, req.Ids);
        return Ok();
    }

    // ── КЕЙСЫ: CRUD ──

    // GET /Admin/CreateCase
    public IActionResult CreateCase() => View(new CreateCaseViewModel());

    // POST /Admin/CreateCase
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCase(CreateCaseViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var meta = await _content.CreateCaseAsync(vm);
        TempData["Success"] = $"Кейс «{meta.TitleRu}» создан";
        return RedirectToAction(nameof(Index), new { section = "case-detail", slug = meta.Slug, lang = "ru" });
    }

    // GET /Admin/EditCaseMeta?id=...
    public async Task<IActionResult> EditCaseMeta(string id, string lang = "ru")
    {
        var c = await _content.GetCaseByIdAsync(id);
        if (c == null) return NotFound();
        return View(new EditCaseMetaViewModel { Case = c, Lang = lang });
    }

    // POST /Admin/EditCaseMeta
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCaseMeta(EditCaseMetaViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        await _content.UpdateCaseMetaAsync(vm.Case);
        TempData["Success"] = "Метаданные кейса обновлены";
        return RedirectToAction(nameof(Index), new { section = "case-detail", slug = vm.Case.Slug, lang = vm.Lang });
    }

    // POST /Admin/DeleteCase
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCase(string id)
    {
        var c = await _content.GetCaseByIdAsync(id);
        await _content.DeleteCaseAsync(id);
        TempData["Success"] = "Кейс удалён";
        return RedirectToAction(nameof(Index), new { section = "cases" });
    }

    // ── ПРЕДПРОСМОТР ──
    public async Task<IActionResult> Preview(string page)
    {
        var lang = page.EndsWith("-en") ? "en" : "ru";
        var blocks = await _content.GetPageBlocksAsync(page);

        // Определяем тип страницы
        string viewName;
        PageViewModel vm;

        if (page.StartsWith("case-") && !page.StartsWith("case-ru") && !page.StartsWith("case-en")
            || (page.StartsWith("case-") && page.Count(c => c == '-') >= 2))
        {
            // Это динамический кейс: case-{slug}-{lang}
            var parts = page.Split('-');
            // slug — всё между "case-" и последним "-ru"/"-en"
            var slug = string.Join("-", parts[1..^1]);
            var meta = await _content.GetCaseBySlugAsync(slug);
            viewName = "~/Views/Case/Detail.cshtml";
            vm = new CaseViewModel { PageName = page, Language = lang, Blocks = blocks, Meta = meta };
        }
        else
        {
            viewName = "~/Views/WhoWeAre/Index.cshtml";
            vm = new PageViewModel { PageName = page, Language = lang, Blocks = blocks };
        }

        ViewBag.IsPreview = true; ViewBag.PreviewPage = page;
        ViewBag.Lang = lang; ViewBag.PageLayoutClass = page.Contains("case") ? "m-light" : "";
        return View(viewName, vm);
    }
}

public class ReorderRequest
{
    public string Page { get; set; } = string.Empty;
    public List<string> Ids { get; set; } = new();
}
