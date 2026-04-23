// AdminController.cs (полный)
using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;
using DrozdovLaw.Models.ViewModels;
using DrozdovLaw.Services;
using Microsoft.AspNetCore.Mvc;

namespace DrozdovLaw.Controllers;

public class AdminController : Controller
{
    private readonly IBlockService _blockService;
    private readonly ISectionService _sectionService;
    private readonly IPreviewService _previewService;
    private readonly ILanguageService _languageService;
    private readonly ITranslationService _translationService;

    public AdminController(IBlockService blockService, ISectionService sectionService, IPreviewService previewService, ILanguageService languageService, ITranslationService translationService)
    {
        _blockService = blockService;
        _sectionService = sectionService;
        _previewService = previewService;
        _languageService = languageService;
        _translationService = translationService;
    }

    public async Task<IActionResult> Languages()
    {
        var languages = await _languageService.GetAllAsync();
        return View(languages);
    }
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не выбран.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            return BadRequest("Недопустимый тип файла.");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pictures");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"pictures/{uniqueFileName}";
        return Ok(new { path = relativePath });
    }
    public async Task<IActionResult> CreateLanguage()
    {
        var languages = await _translationService.GetSupportedLanguagesAsync();
        ViewBag.SupportedLanguages = languages;
        return View(new Language());
    }

    // POST: /Admin/CreateLanguage
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateLanguage(Language language)
    {
        if (ModelState.IsValid)
        {
            await _languageService.CreateAsync(language);
            TempData["Success"] = $"Язык '{language.Name}' добавлен. Страницы для разделов созданы.";
            return RedirectToAction(nameof(Languages));
        }
        return View(language);
    }


    // POST: /Admin/DeleteLanguage/{code}
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLanguage(string code)
    {
        await _languageService.DeleteAsync(code);
        TempData["Success"] = "Язык удалён.";
        return RedirectToAction(nameof(Languages));
    }
    private async Task<int?> GetSectionIdFromSlugAsync(string? slug)
    {
        if (string.IsNullOrEmpty(slug)) return null;
        var section = await _sectionService.GetSectionBySlugAsync(slug);
        return section?.Id;
    }

    public async Task<IActionResult> Index(string section = "cases", string? pageName = null,
                                        string? slug = null, string lang = "ru")
    {
        var vm = new AdminIndexViewModel { ActiveLang = lang, ActiveSection = section };
        var allLanguages = await _languageService.GetAllAsync();

        switch (section)
        {
            case "cases":
                vm.Sections = await _sectionService.GetAllSectionsAsync(onlyPublished: false);
                break;

            case "case-detail" when slug != null:
                var sec = await _sectionService.GetSectionBySlugAsync(slug);
                vm.SelectedSection = sec;
                // Ищем или создаём страницу для case
                if (sec != null)
                {
                    // Передаём sectionId
                    vm.SelectedPage = await _blockService.GetPageAsync("case", lang, sec.Id);
                    if (vm.SelectedPage == null)
                    {
                        // Создаем страницу, если её нет
                        var displayName = sec.Pages?.FirstOrDefault(p => p.LanguageCode == lang)?.Name ?? sec.Slug;
                        var pageId = await _blockService.GetOrCreatePageIdAsync("case", lang, displayName, sec.Id);
                        vm.SelectedPage = await _blockService.GetPageAsync("case", lang, sec.Id);
                    }
                    vm.Blocks = await _blockService.GetPageBlocksAsync("case", lang, sec.Id);
                }
                vm.Sections = await _sectionService.GetAllSectionsAsync(onlyPublished: false);
                break;

            case "whoweare":
                // Создаем/получаем страницу WhoWeAre
                var displayNameWhowe = lang == "ru" ? "Кто мы" : "Who we are";
                await _blockService.GetOrCreatePageIdAsync("whoweare", lang, displayNameWhowe, null);
                vm.SelectedPage = await _blockService.GetPageAsync("whoweare", lang, null);
                vm.SelectedPageName = "whoweare";
                vm.Blocks = await _blockService.GetPageBlocksAsync("whoweare", lang, null);
                vm.AvailableLanguagesForStatic = allLanguages;
                break;
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> EditBlock(int id, string returnPage, string lang = "ru",
                                               string section = "cases", string? slug = null)
    {
        var block = await _blockService.GetBlockByIdAsync(id);
        if (block == null) return NotFound();

        var styles = await _blockService.GetAllStylesAsync();
        ViewBag.Styles = styles;
        ViewBag.Section = section;
        ViewBag.Slug = slug;

        var vm = new AdminEditViewModel
        {
            Id = block.Id,
            Content = block.Content,
            ExtraAttribute = block.ExtraAttribute,
            StyleId = block.StyleId,
            ReturnPage = returnPage,
            Lang = lang
        };
        return View("EditBlock", vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBlock(AdminEditViewModel vm, string section = "cases", string? slug = null)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Styles = await _blockService.GetAllStylesAsync();
            return View("EditBlock", vm);
        }

        await _blockService.UpdateBlockAsync(vm.Id, vm.Content, vm.ExtraAttribute, vm.StyleId);
        TempData["Success"] = "Блок сохранён";
        TempData["ShowPreview"] = "true";
        return RedirectToAction(nameof(Index), new { section, slug, page = vm.ReturnPage, lang = vm.Lang });
    }

    [HttpGet]
    public async Task<IActionResult> CreateBlock(string page, string lang = "ru", string section = "cases", string? slug = null)
    {
        var styles = await _blockService.GetAllStylesAsync();
        ViewBag.Styles = styles;
        ViewBag.Section = section;
        ViewBag.Slug = slug;

        var vm = new AdminEditViewModel
        {
            ReturnPage = page,
            Lang = lang,
            StyleId = styles.FirstOrDefault()?.Id ?? 0
        };
        return View("EditBlock", vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBlock(AdminEditViewModel vm, string section = "cases", string? slug = null)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Styles = await _blockService.GetAllStylesAsync();
            return View("EditBlock", vm);
        }

        var styles = await _blockService.GetAllStylesAsync();
        var style = styles.First(s => s.Id == vm.StyleId);
        int? sectionId = (section == "case-detail") ? await GetSectionIdFromSlugAsync(slug) : null;
        await _blockService.CreateBlockAsync(vm.ReturnPage, vm.Lang, vm.ReturnPage, style.Name, vm.Content, vm.ExtraAttribute, sectionId);

        TempData["Success"] = "Блок создан";
        return RedirectToAction(nameof(Index), new { section, slug, page = vm.ReturnPage, lang = vm.Lang });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBlock(int id, string returnPage, string lang = "ru",
                                                 string section = "cases", string? slug = null)
    {
        await _blockService.DeleteBlockAsync(id);
        TempData["Success"] = "Блок удалён";
        return RedirectToAction(nameof(Index), new { section, slug, page = returnPage, lang });
    }

    [HttpPost]
    public async Task<IActionResult> ReorderBlocks([FromBody] ReorderRequest req)
    {
        int? sectionId = (req.Section == "case-detail" && !string.IsNullOrEmpty(req.Slug))
            ? await GetSectionIdFromSlugAsync(req.Slug)
            : null;
        await _blockService.ReorderBlocksAsync(req.Page, req.Lang, req.Ids, sectionId);
        return Ok();
    }

    // --- Section actions ---
    public async Task<IActionResult> CreateSection()
    {
        ViewBag.Languages = await _languageService.GetAllAsync();
        return View(new CreateSectionViewModel());
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSection(CreateSectionViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Languages = await _languageService.GetAllAsync();
            return View(vm);
        }

        try
        {
            var section = await _sectionService.CreateSectionWithAutoTranslationAsync(vm);
            TempData["Success"] = $"Раздел «{vm.BaseTitle}» создан и переведён на все языки";
            return RedirectToAction(nameof(Index), new { section = "case-detail", slug = section.Slug, lang = vm.BaseLanguageCode });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.Languages = await _languageService.GetAllAsync();
            return View(vm);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CopyAndTranslateBlocks(string slug, string sourceLang, string targetLang)
    {
        try
        {
            await _sectionService.CopyAndTranslateSectionBlocksAsync(slug, sourceLang, targetLang);
            TempData["Success"] = $"Блоки скопированы и переведены с {sourceLang.ToUpper()} на {targetLang.ToUpper()}.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index), new { section = "case-detail", slug, lang = targetLang });
    }
    [HttpGet]
    public async Task<IActionResult> EditSection(int id, string lang = "ru")
    {
        var section = await _sectionService.GetSectionByIdAsync(id);
        if (section == null) return NotFound();

        var page = await _sectionService.GetSectionPageAsync(section.Slug, lang);
        if (page == null)
        {
            page = new Page { SystemName = "case", Name = "case", LanguageCode = lang, SectionId = section.Id };
        }

        return View(new EditSectionViewModel
        {
            Section = section,
            Page = page,
            Lang = lang
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSection(EditSectionViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        await _sectionService.UpdateSectionAsync(vm.Section);
        await _blockService.UpdatePageAsync(vm.Page);

        TempData["Success"] = "Изменения сохранены";
        return RedirectToAction(nameof(Index), new { section = "case-detail", slug = vm.Section.Slug, lang = vm.Lang });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSection(int id)
    {
        var section = await _sectionService.GetSectionByIdAsync(id);
        if (section == null) return NotFound();

        await _sectionService.DeleteSectionAsync(id);
        TempData["Success"] = "Раздел удалён";
        return RedirectToAction(nameof(Index), new { section = "cases" });
    }

    [HttpGet]
    public async Task<IActionResult> EditPage(string pageName, string lang = "ru", string section = "cases", string? slug = null)
    {
        var page = await _blockService.GetPageAsync(pageName, lang);
        if (page == null) return NotFound();
        ViewBag.Section = section;
        ViewBag.Slug = slug;
        return View(page);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPage(Page page, string section = "cases", string? slug = null)
    {
        if (!ModelState.IsValid) return View(page);
        await _blockService.UpdatePageAsync(page);
        TempData["Success"] = "Страница обновлена";
        return RedirectToAction(nameof(Index), new { section, slug, page = page.SystemName, lang = page.LanguageCode });
    }

    public async Task<IActionResult> Preview(string pageName, string lang, string? slug = null, string section = "cases")
    {
        int? sectionId = (section == "case-detail" && !string.IsNullOrEmpty(slug))
            ? await GetSectionIdFromSlugAsync(slug)
            : null;

        var (viewName, viewModel) = await _previewService.GetPreviewAsync(pageName, lang, sectionId);

        ViewBag.IsPreview = true;
        ViewBag.PreviewPage = pageName;
        ViewBag.Lang = lang;
        ViewBag.PageLayoutClass = pageName == "case" ? "m-light" : "";
        return View(viewName, viewModel);
    }
}

public class ReorderRequest
{
    public string Page { get; set; } = string.Empty;
    public string Lang { get; set; } = "ru";
    public string? Slug { get; set; }
    public string Section { get; set; } = "cases";
    public List<int> Ids { get; set; } = new();
}