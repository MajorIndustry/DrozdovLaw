using DrozdovLaw.Models;

namespace DrozdovLaw.Interfaces;

public interface ICaseTemplateBuilder
{
    List<ContentBlock> BuildTemplateBlocks(string pageName, string slug, string lang, CreateCaseViewModel vm);
}