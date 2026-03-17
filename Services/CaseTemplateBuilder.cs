using DrozdovLaw.Interfaces;
using DrozdovLaw.Models;

namespace DrozdovLaw.Services;

public class CaseTemplateBuilder : ICaseTemplateBuilder
{
    public List<ContentBlock> BuildTemplateBlocks(string pageName, string slug, string lang, CreateCaseViewModel vm)
    {
        bool ru = lang == "ru";
        var title = ru ? vm.TitleRu : vm.TitleEn;
        var location = ru ? vm.LocationRu : vm.LocationEn;
        int o = 0;
        ContentBlock B(string style, string content, string? extra = null) =>
            new()
            {
                PageName = pageName,
                Style = style,
                Content = content,
                ExtraAttribute = extra,
                Order = ++o,
                UpdatedAt = DateTime.UtcNow
            };

        return new List<ContentBlock>
        {
            B("breadcrumb", ru ? "Главная / Кейсы" : "Home / Cases"),
            B("e-dots",     "#B8B399,#1E6F89"),
            B("h1",         title),
            B("e-id",       ru ? "Сообщение № —/—" : "Communication No. —/—"),
            B("e-details__type", ru ? "Статус дела" : "Case status", "#B8B399"),
            B("e-details__loc",  location, "images/flags/England.png"),
            B("e-details__def",  ru ? "Тип: укажите тип дела" : "Type: specify case type"),
            B("p",          ru ? "Введите описание дела." : "Enter case description."),
            B("h2",         ru ? "Раздел 1" : "Section 1"),
            B("p",          ru ? "Текст первого раздела." : "First section text."),
            B("blockquote", ru ? "Ключевая цитата по делу." : "Key case quote."),
            B("h2",         ru ? "Раздел 2" : "Section 2"),
            B("p",          ru ? "Текст второго раздела." : "Second section text."),
            B("h3",         ru ? "Работа по делу:" : "Work in the case:"),
            B("ul",         ru ? "Сбор информации|Подготовка документов|Защита в суде"
                               : "Information gathering|Document preparation|Court representation"),
            B("small",      ru ? "Источник: укажите источник информации."
                               : "Source: specify information source."),
            B("person",     ru ? "ДРОЗДОВ Вадим" : "DROZDOV Vadim",
                            ru ? "pictures/person-1.jpg|Адвокат" : "pictures/person-1.jpg|Lawyer"),
            B("note-title", ru ? "Пояснение" : "Explanation"),
            B("note-text",  ru ? "Добавьте пояснение к делу." : "Add case explanation."),
            B("decision-title", ru ? "Решение" : "Decision"),
            B("decision-text",  ru ? "Укажите итог дела." : "Specify case outcome."),
            B("case-link",  "https://"),
            B("docs-title", ru ? "Документы по делу" : "Case documents"),
            B("doc-item",   ru ? "Решение судебного дела" : "Court decision", ru ? "Документ PDF" : "PDF Document"),
            B("tags-title", ru ? "Теги" : "Tags"),
            B("tag",        "Tag 1", "#D5A968"),
            B("section-title", ru ? "ПОХОЖИЕ КЕЙСЫ" : "SIMILAR CASES"),
        };
    }
}