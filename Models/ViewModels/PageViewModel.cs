namespace DrozdovLaw.Models.ViewModels
{
    public class PageViewModel
    {
        public Page Page { get; set; } = new();
        public string Language { get; set; } = "ru";
        public List<ContentBlock> Blocks { get; set; } = new();
        public List<Section> Sections { get; set; } = new();
    }
}
