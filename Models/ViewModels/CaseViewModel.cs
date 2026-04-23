namespace DrozdovLaw.Models.ViewModels
{
    public class CaseViewModel : PageViewModel
    {
        public Section Section { get; set; } = new();
        public List<Section> SimilarSections { get; set; } = new();
    }
}
