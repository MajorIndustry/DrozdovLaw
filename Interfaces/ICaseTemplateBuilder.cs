using DrozdovLaw.Models;
using DrozdovLaw.Models.ViewModels;

namespace DrozdovLaw.Interfaces;

public interface ICaseTemplateBuilder
{
    IEnumerable<ContentBlock> BuildTemplateBlocks(int pageId, string slug, string lang, CreateSectionViewModel model);
}