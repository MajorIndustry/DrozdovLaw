namespace DrozdovLaw.Models.ViewModels
{
    public class CasesListViewModel
    {
        public string Language { get; set; } = "ru";
        public List<Section> Sections { get; set; } = new();
    }
}
