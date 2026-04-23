using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;
using DrozdovLaw.Models.ViewModels;

namespace DrozdovLaw.Services;

public class CaseTemplateBuilder : ICaseTemplateBuilder
{
    public IEnumerable<ContentBlock> BuildTemplateBlocks(int pageId, string slug, string lang, CreateSectionViewModel model)
    {
        // Определяем, на каком языке генерируем шаблон
        bool isRussian = lang == "ru";
        bool isEnglish = lang == "en";

        // Заголовок теперь берём из BaseTitle (предполагаем, что он уже переведён или будет переведён)
        // Для базового языка используем BaseTitle напрямую, для остальных – переведённое значение
        string title = lang == model.BaseLanguageCode ? model.BaseTitle : model.BaseTitle; // В реальности здесь будет переведённая строка, но в шаблоне можно оставить как есть, т.к. перевод происходит после создания блоков

        int order = 0;

        ContentBlock CreateBlock(string styleName, string content, string? extra = null)
        {
            return new ContentBlock
            {
                PageId = pageId,
                Order = ++order,
                StyleName = styleName,
                Content = content,
                ExtraAttribute = extra,
                UpdatedAt = DateTime.UtcNow
            };
        }

        // Для контента используем язык, на котором генерируем страницу
        yield return CreateBlock("breadcrumb", isRussian ? "Главная / Кейсы" : "Home / Cases");
        yield return CreateBlock("e-dots", null, "#B8B399,#1E6F89");
        yield return CreateBlock("h1", title);
        yield return CreateBlock("e-id", isRussian ? "Сообщение № —/—" : "Communication No. —/—");
        yield return CreateBlock("e-details__type", isRussian ? "Статус дела" : "Case status", model.StatusColor);
        yield return CreateBlock("e-details__loc", "", model.FlagImage);
        yield return CreateBlock("e-details__def", isRussian ? "Тип: укажите тип дела" : "Type: specify case type");
        yield return CreateBlock("p", isRussian ? "Введите описание дела." : "Enter case description.");
        yield return CreateBlock("h2", isRussian ? "Раздел 1" : "Section 1");
        yield return CreateBlock("p", isRussian ? "Текст первого раздела." : "First section text.");
        yield return CreateBlock("blockquote", isRussian ? "Ключевая цитата по делу." : "Key case quote.");
        yield return CreateBlock("h2", isRussian ? "Раздел 2" : "Section 2");
        yield return CreateBlock("p", isRussian ? "Текст второго раздела." : "Second section text.");
        yield return CreateBlock("h3", isRussian ? "Работа по делу:" : "Work in the case:");
        yield return CreateBlock("ul", isRussian ? "Сбор информации|Подготовка документов|Защита в суде"
                                                : "Information gathering|Document preparation|Court representation");
        yield return CreateBlock("small", isRussian ? "Источник: укажите источник информации."
                                                    : "Source: specify information source.");
        yield return CreateBlock("person", isRussian ? "ДРОЗДОВ Вадим" : "DROZDOV Vadim",
                                isRussian ? "pictures/person-1.jpg|Адвокат" : "pictures/person-1.jpg|Lawyer");
        yield return CreateBlock("note-title", isRussian ? "Пояснение" : "Explanation");
        yield return CreateBlock("note-text", isRussian ? "Добавьте пояснение к делу." : "Add case explanation.");
        yield return CreateBlock("decision-title", isRussian ? "Решение" : "Decision");
        yield return CreateBlock("decision-text", isRussian ? "Укажите итог дела." : "Specify case outcome.");
        yield return CreateBlock("case-link", "https://");
        yield return CreateBlock("docs-title", isRussian ? "Документы по делу" : "Case documents");
        yield return CreateBlock("doc-item", isRussian ? "Решение судебного дела" : "Court decision",
                                isRussian ? "Документ PDF" : "PDF Document");
        yield return CreateBlock("tags-title", isRussian ? "Теги" : "Tags");
        yield return CreateBlock("tag", "Tag 1", "#D5A968");
        yield return CreateBlock("section-title", isRussian ? "ПОХОЖИЕ КЕЙСЫ" : "SIMILAR CASES");
    }
}