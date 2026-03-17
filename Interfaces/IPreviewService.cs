namespace DrozdovLaw.Interfaces;

public interface IPreviewService
{
    Task<(string viewName, object viewModel)> GetPreviewAsync(string page);
}