# Drozdov Law Office — Веб-приложение

Платформа для управления контентом юридического сайта **Vadim Drozdov Law Office**. Реализована на **ASP.NET Core MVC** (C#) с использованием **Entity Framework Core** и **MS SQL Server**.

Приложение позволяет управлять кейсами (делами), статическими страницами, локализованным контентом и медиа-блоками через админ-панель. Поддерживает мультиязычность с автоматическим переводом через **Google Cloud Translation API**.

---

## Оглавление

- [Возможности](#возможности)
- [Архитектура](#архитектура)
- [Технологический стек](#технологический-стек)
- [Установка и запуск](#установка-и-запуск)
  - [Требования](#требования)
  - [Конфигурация](#конфигурация)
  - [Сборка и миграции](#сборка-и-миграции)
- [Использование](#использование)
- [Структура проекта](#структура-проекта)
- [API и сервисы](#api-и-сервисы)
- [Лицензия](#лицензия)

---

## Возможности

- **Мультиязычность** (русский, английский и любые дополнительные языки, добавляемые через админку)
- **Управление кейсами (разделами)**
  - Создание / редактирование / удаление разделов
  - Автоматический перевод контента на все языки при создании
  - Копирование и перевод блоков между языками вручную
- **Гибкая система контент-блоков**
  - Поддержка более 20 стилей (заголовки, текст, списки, цитаты, карточки людей, ссылки, документы, теги, хлебные крошки, цветные точки и т.д.)
  - Drag-and-drop изменение порядка блоков
  - Динамический выбор дополнительных атрибутов (цвет, изображение, URL, тип документа) в зависимости от стиля блока
- **Статические страницы** (например, «Кто мы») с мультиязычным контентом
- **Админ-панель** с удобным интерфейсом, переключением языков и предпросмотром
- **Управление языками** через админку (добавление / удаление с автоматическим созданием переведённых страниц)
- **Google Cloud Translation API** для автоматического перевода

---

## Архитектура

Проект построен по многослойной архитектуре, разделённой на зоны ответственности:

- **Models** — сущности БД (`Section`, `Page`, `ContentBlock`, `BlockStyle`, `Language`)
- **Data** — контекст `AppDbContext` (Entity Framework Core)
- **Interfaces** — контракты сервисов (`IBlockService`, `ISectionService`, `IPreviewService`, `ILanguageService`, `ITranslationService`, `ICaseTemplateBuilder`)
- **Services** — бизнес-логика (`BlockService`, `SectionService`, `PreviewService`, `LanguageService`, `TranslationService`, `CaseTemplateBuilder`)
- **Controllers** — `AdminController`, `CaseController`, `WhoWeAreController`, `HomeController`
- **Views** — Razor-представления (админка, публичные страницы)

Применяются **принципы SOLID**, внедрение зависимостей (DI), все операции с базой данных вынесены в сервисы.

---

## Технологический стек

- **Backend**: ASP.NET Core MVC (.NET 8+)
- **ORM**: Entity Framework Core
- **База данных**: Microsoft SQL Server
- **Переводы**: Google Cloud Translation API v2
- **Аутентификация**: (опционально) ASP.NET Core Identity (не включена в текущую версию)
- **Хранение изображений**: локальная папка `wwwroot/pictures`
- **Контейнеризация**: (опционально) Docker

---

## Установка и запуск

### Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB, Express или полноценный сервер)
- [Git](https://git-scm.com)
- API-ключ Google Cloud Translation (для автоматического перевода)

### Конфигурация

1. **Клонируйте репозиторий**:
   ```bash
   git clone https://github.com/MajorIndustry/DrozdovLaw.git
   cd DrozdovLaw
2.  **Настройте строку подключения в appsettings.json:**
    ```bash
    {
    "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DrozdovLawDb;Trusted_Connection=True;TrustServerCertificate=True;"
    }
    }
3.  **Настройте Google Translate API (если планируете использовать авто-перевод):**
    - Получите API-ключ в Google Cloud Console
    - Включите Cloud Translation API
    - Добавьте ключ в appsettings.json:
    ```bash
    {
    "GoogleTranslate": {
    "ApiKey": "ВАШ_API_КЛЮЧ"
    }
    }
4.  **Установите инструменты EF Core (если ещё не установлены):**
    ```bash
    dotnet tool install --global dotnet-ef
