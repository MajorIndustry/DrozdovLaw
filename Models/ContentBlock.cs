using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrozdovLaw.Models;

[Table("ContentBlocks")]
public class ContentBlock
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PageId { get; set; }

    [ForeignKey(nameof(PageId))]
    public Page Page { get; set; } = null!;

    public int Order { get; set; }

    [Required]
    public int StyleId { get; set; }

    [ForeignKey(nameof(StyleId))]
    public BlockStyle Style { get; set; } = null!;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string? ExtraAttribute { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public string StyleName { get; set; } = string.Empty;
}