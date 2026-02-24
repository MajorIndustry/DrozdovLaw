namespace DrozdovLaw.Models;

public class PageViewModel
{
    public string PageName { get; set; } = string.Empty;
    public string Language { get; set; } = "ru";
    public List<ContentBlock> Blocks { get; set; } = new();
}

public class CaseViewModel : PageViewModel
{
    public CaseMeta? Meta { get; set; }
}

// Список кейсов
public class CasesListViewModel
{
    public string Language { get; set; } = "ru";
    public List<CaseMeta> Cases { get; set; } = new();
}

// ── ADMIN ──

public class AdminIndexViewModel
{
    public string ActiveLang { get; set; } = "ru";
    public string ActiveSection { get; set; } = "cases"; // "cases" | "whoweare" | "case-detail"

    // Список кейсов (для секции cases)
    public List<CaseMeta> Cases { get; set; } = new();

    // Блоки конкретной страницы (для редактирования)
    public string? SelectedPage { get; set; }
    public List<ContentBlock> Blocks { get; set; } = new();

    // Метаданные редактируемого кейса (если это кейс)
    public CaseMeta? SelectedCase { get; set; }

    // Все страницы whoweare
    public List<string> StaticPages { get; set; } = new() { "whoweare-ru", "whoweare-en" };

    public List<string> PagesForLang(string lang) =>
        StaticPages.Where(p => p.EndsWith($"-{lang}")).ToList();
}

public class AdminEditViewModel
{
    public ContentBlock Block { get; set; } = new();
    public string ReturnPage { get; set; } = string.Empty;
    public string Lang { get; set; } = "ru";

    public static List<(string Value, string Group, string Description)> AvailableStyles => new()
    {
        ("h1",              "Заголовки",     "Заголовок 1"),
        ("h2",              "Заголовки",     "Заголовок 2"),
        ("h3",              "Заголовки",     "Заголовок 3"),
        ("h4",              "Заголовки",     "Заголовок 4"),
        ("h5",              "Заголовки",     "Заголовок 5"),
        ("p",               "Текст",         "Обычный абзац"),
        ("p-large",         "Текст",         "Крупный абзац"),
        ("blockquote",      "Текст",         "Цитата"),
        ("small",           "Текст",         "Мелкий текст / сноска"),
        ("ul",              "Списки",        "Маркированный список (пункты через |)"),
        ("ol",              "Списки",        "Нумерованный список (пункты через |)"),
        ("e-dots",          "Шапка статьи",  "Цветные точки (цвета через запятую)"),
        ("e-id",            "Шапка статьи",  "Номер/ID дела"),
        ("e-details__type", "Шапка статьи",  "Тип результата (ExtraAttribute = CSS-цвет)"),
        ("e-details__loc",  "Шапка статьи",  "Локация + флаг (ExtraAttribute = путь к флагу)"),
        ("e-details__def",  "Шапка статьи",  "Тип дела"),
        ("person",          "Правая колонка","Адвокат (ExtraAttribute = фото|должность)"),
        ("note-title",      "Правая колонка","Заголовок пояснения"),
        ("note-text",       "Правая колонка","Текст пояснения"),
        ("decision-title",  "Решение",       "Заголовок 'Решение'"),
        ("decision-text",   "Решение",       "Текст решения"),
        ("case-link",       "Решение",       "Ссылка на дело (URL)"),
        ("docs-title",      "Документы",     "Заголовок 'Документы по делу'"),
        ("doc-item",        "Документы",     "Документ PDF (ExtraAttribute = тип файла)"),
        ("tags-title",      "Теги",          "Заголовок 'Теги'"),
        ("tag",             "Теги",          "Тег (ExtraAttribute = CSS-цвет фона)"),
        ("breadcrumb",      "Навигация",     "Хлебные крошки (части через /)"),
        ("section-title",   "Навигация",     "Заголовок секции"),
    };
}

// Форма создания нового кейса
public class CreateCaseViewModel
{
    public string Slug { get; set; } = string.Empty;
    public string TitleRu { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string StatusColor { get; set; } = "#B4D4EB";
    public string StatusRu { get; set; } = "Завершено успешно";
    public string StatusEn { get; set; } = "Successfully completed";
    public string LocationRu { get; set; } = string.Empty;
    public string LocationEn { get; set; } = string.Empty;
    public string SummaryRu { get; set; } = " ";
    public string SummaryEn { get; set; } = " ";
    public string FlagImage { get; set; } = "images/flag-sw.png";
}

// Форма редактирования метаданных кейса
public class EditCaseMetaViewModel
{
    public CaseMeta Case { get; set; } = new();
    public string Lang { get; set; } = "ru";
}
