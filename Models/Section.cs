using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrozdovLaw.Models;

/// <summary>
/// Раздел (кейс), объединяющий страницы на разных языках.
/// Содержит общие для всех языков метаданные.
/// </summary>
[Table("Sections")]
public class Section
{
    [Key]
    public int Id { get; set; }

    /// <summary>Уникальный URL-идентификатор, например "kk-vs-switzerland"</summary>
    [Required]
    [MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    /// <summary>Путь к изображению флага (общий для всех языков)</summary>
    [MaxLength(500)]
    public string FlagImage { get; set; } = string.Empty;

    /// <summary>CSS-цвет статуса (например, #B4D4EB)</summary>
    [MaxLength(20)]
    public string StatusColor { get; set; } = "#B4D4EB";

    /// <summary>Дата создания раздела</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Опубликован ли раздел (влияет на отображение в списках)</summary>
    public bool IsPublished { get; set; } = true;

    /// <summary>Страницы, принадлежащие этому разделу (обычно две: case-{slug}-ru и case-{slug}-en)</summary>
    public ICollection<Page> Pages { get; set; } = new List<Page>();
}