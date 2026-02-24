# Drozdov Law — ASP.NET Core MVC

## Структура проекта

```
DrozdovLaw/
├── Controllers/
│   ├── CaseController.cs        # Страница "Кейс" (RU + EN)
│   ├── WhoWeAreController.cs    # Страница "Кто мы" (RU + EN)
│   ├── AdminController.cs       # Панель администратора
│   └── HomeController.cs        # Редирект на главную
│
├── Models/
│   ├── ContentBlock.cs          # Модель текстового блока
│   └── ViewModels.cs            # ViewModel для страниц и админки
│
├── Services/
│   └── ContentService.cs        # Чтение/запись JSON, CRUD операции
│
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml       # Общий шаблон (хэдер + футер)
│   │   └── _ContentBlock.cshtml # Partial view для рендеринга блока по стилю
│   ├── Case/
│   │   └── Index.cshtml         # Страница кейса
│   ├── WhoWeAre/
│   │   └── Index.cshtml         # Страница "Кто мы"
│   └── Admin/
│       ├── Index.cshtml         # Список блоков страницы
│       └── Edit.cshtml          # Редактор блока с превью
│
└── Data/
    └── content.json             # База данных (все текстовые блоки)
```

## Запуск

```bash
cd DrozdovLaw
dotnet run
```

Приложение запустится на http://localhost:5000

## URL-адреса

| URL                          | Описание                        |
|------------------------------|---------------------------------|
| `/Case?lang=ru`              | Кейс на русском                 |
| `/Case?lang=en`              | Case in English                 |
| `/WhoWeAre?lang=ru`          | Кто мы (RU)                     |
| `/WhoWeAre?lang=en`          | Who we are (EN)                 |
| `/Admin`                     | Панель администратора           |
| `/Admin?page=case-ru`        | Блоки страницы case-ru          |
| `/Admin?page=case-en`        | Блоки страницы case-en          |
| `/Admin?page=whoweare-ru`    | Блоки страницы whoweare-ru      |
| `/Admin?page=whoweare-en`    | Блоки страницы whoweare-en      |

## Стили текстовых блоков

Все стили выявлены из HTML-файлов case.html, caseEn.html, whoweare.html, whoweareEn.html:

| Стиль             | HTML-элемент               | Описание                              |
|-------------------|---------------------------|---------------------------------------|
| `h1`              | `<h1>`                    | Заголовок первого уровня              |
| `h2`              | `<h2>`                    | Заголовок второго уровня             |
| `h3`              | `<h3>`                    | Заголовок третьего уровня            |
| `h4`              | `<h4>`                    | Заголовок четвёртого уровня          |
| `h5`              | `<h5>`                    | Заголовок пятого уровня              |
| `p`               | `<p>`                     | Обычный абзац                         |
| `p-large`         | `<p class="p-large">`     | Крупный абзац (секция e-text-large)  |
| `blockquote`      | `<blockquote>`            | Цитата                                |
| `small`           | `<small>`                 | Мелкий текст / сноска                |
| `ul`              | `<ul><li>...</li></ul>`   | Маркированный список (через `\|`)    |
| `ol`              | `<ol><li>...</li></ol>`   | Нумерованный список (через `\|`)     |
| `e-id`            | `<div class="e-id">`      | ID/номер дела                         |
| `e-details__type` | `<div class="e-details__type">` | Тип результата (с цветом в ExtraAttribute) |
| `e-details__loc`  | `<div class="e-details__loc">`  | Локация/юрисдикция (флаг в ExtraAttribute) |
| `e-details__def`  | `<div class="e-details__def">`  | Определение/тип дела              |
| `note-title`      | `<h5>` внутри `.section-article__note` | Заголовок пояснения        |
| `note-text`       | `<p>` внутри `.section-article__note`  | Текст пояснения           |
| `breadcrumb`      | `.breadcrumbs ul li`      | Хлебные крошки (через `/`)           |
| `section-title`   | `<h1>` секции             | Заголовок секции страницы            |
| `e-dots`          | `<div class="e-dots">`    | Цветные точки (цвета через `,`)      |

## Формат content.json

```json
{
  "blocks": [
    {
      "id": "уникальный-id",
      "pageName": "case-ru",          // case-ru | case-en | whoweare-ru | whoweare-en
      "order": 1,                      // порядок на странице
      "style": "h1",                   // стиль из таблицы выше
      "content": "К.К. против Швейцарии",
      "extraAttribute": null,          // цвет или путь к флагу (для e-details)
      "updatedAt": "2025-01-01T00:00:00Z"
    }
  ]
}
```

## Возможности Admin-панели

- 📋 **Список блоков** по страницам с порядком, стилем и превью текста
- ✏️ **Редактирование** любого блока с живым превью стиля
- ➕ **Добавление** новых блоков
- 🗑 **Удаление** блоков
- ☰ **Drag & Drop** для изменения порядка блоков (сохраняется через AJAX)
- 🏷 **Быстрый выбор стиля** кликом по бейджу

## CSS

Подключите `css/styles.css` из оригинальных файлов проекта в папку `wwwroot/css/`.
