using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrozdovLaw.Models;

/// <summary>
/// Страница сайта. Может быть статической (whoweare-ru) или принадлежать разделу (case-{slug}-ru).
/// </summary>
[Table("Pages")]
public class Page
{
    [Key]
    public int Id { get; set; }

    /// <summary>Системное имя: "case", "whoweare"</summary>
    [Required, MaxLength(50)]
    public string SystemName { get; set; } = string.Empty;

    /// <summary>Отображаемый заголовок страницы (на конкретном языке)</summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(5)]
    public string LanguageCode { get; set; } = string.Empty;
    [BindNever]
    [ValidateNever]
    [ForeignKey(nameof(LanguageCode))]
    public Language Language { get; set; } = null!;

    /// <summary>ID раздела, если страница принадлежит кейсу (NULL для статических страниц)</summary>
    public int? SectionId { get; set; }

    [ForeignKey(nameof(SectionId))]
    public Section? Section { get; set; }

    /// <summary>Статус на языке страницы (например, "Завершено успешно")</summary>
    [MaxLength(200)]
    public string? Status { get; set; }

    /// <summary>Краткое описание на языке страницы</summary>
    public string? Summary { get; set; }

    /// <summary>Блоки контента, принадлежащие этой странице</summary>
    public ICollection<ContentBlock> ContentBlocks { get; set; } = new List<ContentBlock>();
}