namespace DrozdovLaw.Interfaces;

public interface IPreviewService
{
    Task<(string viewName, object viewModel)> GetPreviewAsync(string pageName, string lang, int? sectionId = null);
}