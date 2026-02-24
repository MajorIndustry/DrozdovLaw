namespace DrozdovLaw.Models;

/// <summary>
/// Один текстовый/медиа блок страницы.
/// PageName формат:
///   - "whoweare-ru" / "whoweare-en"  — статические страницы
///   - "case-{slug}-ru" / "case-{slug}-en" — динамические кейсы
/// </summary>
public class ContentBlock
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PageName { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Style { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ExtraAttribute { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Метаданные кейса — отдельная запись для списка и навигации
/// </summary>
public class CaseMeta
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Slug { get; set; } = string.Empty;        // уникальный URL-идентификатор, напр. "kk-vs-switzerland"

    // Отображаемые данные (берём из блоков, но дублируем для быстрого доступа в списках)
    public string TitleRu { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string StatusColor { get; set; } = "#B4D4EB";    // цвет статуса на карточке
    public string StatusRu { get; set; } = "Завершено успешно";
    public string StatusEn { get; set; } = "Successfully completed";
    public string LocationRu { get; set; } = string.Empty;
    public string LocationEn { get; set; } = string.Empty;
    public string SummaryRu { get; set; } = string.Empty;
    public string SummaryEn { get; set; } = string.Empty;
    public string FlagImage { get; set; } = string.Empty;   // путь к флагу для карточки

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublished { get; set; } = true;
}

public class ContentData
{
    public List<ContentBlock> Blocks { get; set; } = new();
    public List<CaseMeta> Cases { get; set; } = new();
}
