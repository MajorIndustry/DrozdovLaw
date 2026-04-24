# DrozdovLaw

Веб-сайт юридической компании на **ASP.NET Core MVC (.NET 8)** с поддержкой неограниченного числа языков и **автоматическим переводом контента** через Google Translate API.

## Ключевые возможности

- **Неограниченная мультиязычность** — добавление нового языка через админку автоматически генерирует переведённые копии всех существующих кейсов и статических страниц.
- **Автоперевод через Google Translate API** — при создании нового кейса или нового языка `TranslationService` переводит каждый блок контента, заголовок, статус и аннотацию.
- **Переключатель языков на фронте** — список доступных языков передаётся в `ViewBag.AvailableLanguages` для рендеринга в шапке.
- **Блочная структура контента** — каждая страница состоит из упорядоченных `ContentBlock`-ов с типизированными стилями (`h1`, `blockquote`, `e-details__type`, `person`, `tag` и др.).
- **Шаблонная генерация кейсов** — `CaseTemplateBuilder` создаёт стандартный набор блоков при добавлении нового дела.
- **Административная панель** — полный CRUD для блоков, кейсов и языков, drag-and-drop сортировка блоков, загрузка изображений, превью страниц.

## Технологический стек

- **ASP.NET Core MVC** (.NET 8)
- **Entity Framework Core 8** + **SQL Server**
- **Google Cloud Translation API v2**

## Архитектура

```
DrozdovLaw/
├── Controllers/
│   ├── CaseController.cs          # Список кейсов и детальная страница
│   ├── WhoWeAreController.cs      # Статическая страница "Кто мы"
│   ├── AdminController.cs         # Панель администратора
│   └── HomeController.cs          # Редирект / → /Cases
│
├── Data/
│   ├── AppDbContext.cs            # Контекст EF Core
│   └── JsonMigrator.cs            # Утилита переноса данных из JSON в SQL Server
│
├── Interfaces/                    # Контракты для всех сервисов
│
├── Models/
│   ├── Section.cs                 # Кейс (Slug, FlagImage, StatusColor, IsPublished)
│   ├── Page.cs                    # Языковая версия страницы (SystemName, LanguageCode, Status, Summary)
│   ├── ContentBlock.cs            # Блок контента (Content, Order, StyleId, ExtraAttribute)
│   ├── BlockStyle.cs              # Стиль блока (h1, p, blockquote, e-dots, person, tag …)
│   ├── Language.cs                # Язык (Code PK, Name)
│   └── ViewModels/
│
├── Services/
│   ├── TranslationService.cs      # Google Translate API v2 — перевод и список языков
│   ├── LanguageService.cs         # Управление языками + автоперевод статических страниц
│   ├── SectionService.cs          # Управление кейсами + автоперевод при создании / добавлении языка
│   ├── BlockService.cs            # CRUD блоков и страниц, управление стилями
│   ├── CaseTemplateBuilder.cs     # Генерация шаблонных блоков для нового кейса
│   └── PreviewService.cs          # Получение вьюмодели для превью страницы в админке
│
├── Repositories/
├── Views/
└── wwwroot/                       # CSS, JS, изображения, флаги, pictures/
```

## Схема базы данных

```
Languages ──────────────────────────────────────────────────────────────────────
  Code (PK, varchar 5)   Name

Sections ───────────────────────────────────────────────────────────────────────
  Id (PK)   Slug (unique)   FlagImage   StatusColor   CreatedAt   IsPublished

Pages ──────────────────────────────────────────────────────────────────────────
  Id (PK)   SystemName   Name   LanguageCode (FK→Languages)
  SectionId (FK→Sections, nullable)   Status   Summary

ContentBlocks ──────────────────────────────────────────────────────────────────
  Id (PK)   PageId (FK→Pages)   StyleId (FK→BlockStyles)
  Order   Content   ExtraAttribute   UpdatedAt

BlockStyles ────────────────────────────────────────────────────────────────────
  Id (PK)   Name   Description
```

Статические страницы (например, `whoweare`) имеют `SectionId = NULL`. Кейсы всегда привязаны к конкретной `Section`.

## Механизм автоперевода

### Добавление нового языка

Когда через `LanguageService.CreateAsync` регистрируется новый язык:

1. **Все существующие кейсы** — `SectionService.AddLanguageToAllSectionsAsync` создаёт новую `Page` для каждой секции, копирует и переводит каждый `ContentBlock`.
2. **Все статические страницы** (`whoweare` и др.) — `AddLanguageToStaticPagesAsync` делает то же самое.

### Добавление нового кейса

`SectionService.CreateSectionWithAutoTranslationAsync`:

1. Создаёт `Section` с общими метаданными (slug, флаг, цвет статуса).
2. Создаёт базовую `Page` на выбранном языке.
3. `CaseTemplateBuilder` генерирует стандартный набор блоков для базового языка.
4. Для каждого уже зарегистрированного языка автоматически создаётся переведённая `Page` со своими блоками.

### Ретрансляция блоков кейса

`Admin/CopyAndTranslateBlocks?slug=&sourceLang=&targetLang=` позволяет в любой момент перегенерировать перевод конкретного кейса с любого исходного языка на любой целевой через `SectionService.CopyAndTranslateSectionBlocksAsync`.

### TranslationService

Вызывает `POST https://translation.googleapis.com/language/translate/v2` с API-ключом из конфигурации. При ошибке — тихий fallback к оригинальному тексту. Метод `GetSupportedLanguagesAsync` возвращает полный список языков Google Translate для выбора при добавлении нового языка в UI.

## Быстрый старт

### Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (Express / LocalDB подойдёт)
- API-ключ [Google Cloud Translation](https://cloud.google.com/translate)

### Установка

```bash
git clone https://github.com/MajorIndustry/DrozdovLaw.git
cd DrozdovLaw
```

### Конфигурация `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DrozdovLaw;Trusted_Connection=True;"
  },
  "GoogleTranslate": {
    "ApiKey": "YOUR_GOOGLE_TRANSLATE_API_KEY"
  }
}
```

### Применение миграций и запуск

```bash
dotnet ef database update
dotnet run
```

Приложение будет доступно по адресу `http://localhost:5000`.

### Миграция данных из старого JSON

Если в корне проекта есть `seed-data.json` (данные из предыдущей JSON-версии), раскомментируйте блок авто-миграции в `Program.cs` — `JsonMigrator` перенесёт данные в SQL Server при первом запуске.

## Маршруты

### Публичная часть

| URL | Контроллер | Описание |
|-----|-----------|----------|
| `/` | `HomeController` | Редирект на `/Cases` |
| `/Cases?lang=ru` | `CaseController.List` | Список кейсов на выбранном языке |
| `/Cases/{slug}?lang=ru` | `CaseController.Detail` | Детальная страница кейса |
| `/WhoWeAre?lang=ru` | `WhoWeAreController.Index` | Страница «Кто мы» |

Параметр `lang` принимает любой код из таблицы `Languages`. Список доступных языков передаётся через `ViewBag.AvailableLanguages`.

### Административная панель

| URL | Описание |
|-----|----------|
| `/Admin` | Список всех кейсов |
| `/Admin?section=case-detail&slug={slug}&lang=ru` | Блоки конкретного кейса |
| `/Admin?section=whoweare&lang=ru` | Блоки статической страницы |
| `/Admin/Languages` | Управление языками |
| `/Admin/CreateLanguage` | Добавить язык (список из Google Translate) |
| `/Admin/CreateSection` | Создать новый кейс с автопереводом |
| `/Admin/EditSection/{id}?lang=ru` | Редактировать метаданные кейса |
| `/Admin/EditBlock/{id}` | Редактировать блок |
| `/Admin/CreateBlock` | Создать новый блок |
| `POST /Admin/DeleteBlock` | Удалить блок |
| `POST /Admin/ReorderBlocks` | Сохранить порядок блоков (JSON, drag-and-drop) |
| `POST /Admin/CopyAndTranslateBlocks` | Перегенерировать перевод кейса |
| `POST /Admin/UploadImage` | Загрузить изображение в `wwwroot/pictures/` |
| `/Admin/Preview` | Превью страницы внутри админки |

## Стили контентных блоков

| Стиль | Описание |
|-------|----------|
| `h1` – `h5` | Заголовки |
| `p`, `p-large` | Абзацы |
| `blockquote` | Цитата |
| `small` | Сноска / мелкий текст |
| `ul`, `ol` | Списки (элементы через `\|`) |
| `e-id` | Номер дела |
| `e-details__type` | Статус дела (цвет в `ExtraAttribute`) |
| `e-details__loc` | Локация / юрисдикция (флаг в `ExtraAttribute`) |
| `e-details__def` | Тип / квалификация дела |
| `e-dots` | Цветные точки (цвета через `,`) |
| `person` | Карточка адвоката (фото и должность в `ExtraAttribute`) |
| `note-title`, `note-text` | Пояснение к делу |
| `decision-title`, `decision-text` | Итог / решение |
| `case-link` | Ссылка на официальный документ |
| `doc-item` | Прикреплённый документ (описание в `ExtraAttribute`) |
| `tag` | Тег (цвет в `ExtraAttribute`) |
| `breadcrumb` | Хлебные крошки (элементы через `/`) |
| `section-title` | Заголовок секции (например, «Похожие кейсы») |

## Зависимости

| Пакет | Версия |
|-------|--------|
| `Microsoft.EntityFrameworkCore` | 8.0.0 |
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.0 |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.0 |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.0 |
