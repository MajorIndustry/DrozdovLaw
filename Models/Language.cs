using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrozdovLaw.Models;

[Table("Languages")]
public class Language
{
    [Key]
    [MaxLength(5)]
    public string Code { get; set; } = string.Empty; // "ru", "en"

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty; // "Русский", "English"

    public ICollection<Page> Pages { get; set; } = new List<Page>();
}