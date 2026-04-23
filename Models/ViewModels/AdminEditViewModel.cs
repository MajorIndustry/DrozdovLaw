namespace DrozdovLaw.Models.ViewModels;

public class AdminEditViewModel
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ExtraAttribute { get; set; }
    public int StyleId { get; set; }
    public string ReturnPage { get; set; } = string.Empty;
    public string Lang { get; set; } = "ru";
}