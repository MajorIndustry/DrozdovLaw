using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrozdovLaw.Models;

[Table("BlockStyles")]
public class BlockStyle
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public ICollection<ContentBlock> ContentBlocks { get; set; } = new List<ContentBlock>();
}