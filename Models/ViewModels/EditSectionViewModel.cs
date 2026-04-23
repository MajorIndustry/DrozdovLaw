namespace DrozdovLaw.Models.ViewModels
{
    public class EditSectionViewModel
    {
        public Section Section { get; set; } = new();
        public Page Page { get; set; } = new();
        public string Lang { get; set; } = "ru";
    }
}
