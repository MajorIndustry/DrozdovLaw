namespace DrozdovLaw.Models.ViewModels;

public class AdminIndexViewModel
{
    public string ActiveLang { get; set; } = "ru";
    public string ActiveSection { get; set; } = "cases"; // "cases" | "whoweare" | "case-detail"

    public List<Section> Sections { get; set; } = new();

    public string? SelectedPageName { get; set; } // имя страницы (например "case")
    public Page? SelectedPage { get; set; }        // объект страницы

    public List<ContentBlock> Blocks { get; set; } = new();

    public Section? SelectedSection { get; set; }

    public List<string> StaticPages { get; set; } = new() { "whoweare-ru", "whoweare-en" };

    public List<string> PagesForLang(string lang) =>
        StaticPages.Where(p => p.EndsWith($"-{lang}")).ToList();
    public List<Language>? AvailableLanguagesForStatic { get; set; }
}