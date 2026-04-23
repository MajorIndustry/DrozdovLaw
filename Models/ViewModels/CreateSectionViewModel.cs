using System.ComponentModel.DataAnnotations;

namespace DrozdovLaw.Models.ViewModels
{
    public class CreateSectionViewModel
    {
        [Required(ErrorMessage = "Slug обязателен")]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public string BaseLanguageCode { get; set; } = "ru";  // язык, который заполняет пользователь

        [Required(ErrorMessage = "Название обязательно")]
        public string BaseTitle { get; set; } = string.Empty;

        public string BaseStatus { get; set; } = string.Empty;
        public string BaseSummary { get; set; } = string.Empty;

        public string StatusColor { get; set; } = "#B4D4EB";
        public string FlagImage { get; set; } = "images/flag-sw.png";
    }
}