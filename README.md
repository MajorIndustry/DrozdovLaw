Drozdov Law Office — Веб-приложение
Платформа для управления контентом юридического сайта Vadim Drozdov Law Office. Реализована на ASP.NET Core MVC (C#) с использованием Entity Framework Core и MS SQL Server.

Приложение позволяет управлять кейсами (делами), статическими страницами, локализованным контентом и медиа-блоками через админ-панель. Поддерживает мультиязычность с автоматическим переводом через Google Cloud Translation API.

Оглавление
Возможности

Архитектура

Технологический стек

Установка и запуск

Требования

Конфигурация

Сборка и миграции

Использование

Структура проекта

API и сервисы

Лицензия

Возможности
Мультиязычность (русский, английский и любые дополнительные языки, добавляемые через админку)

Управление кейсами (разделами)

Создание / редактирование / удаление разделов

Автоматический перевод контента на все языки при создании

Копирование и перевод блоков между языками вручную

Гибкая система контент-блоков

Поддержка более 20 стилей (заголовки, текст, списки, цитаты, карточки людей, ссылки, документы, теги, хлебные крошки, цветные точки и т.д.)

Drag-and-drop изменение порядка блоков

Динамический выбор дополнительных атрибутов (цвет, изображение, URL, тип документа) в зависимости от стиля блока

Статические страницы (например, «Кто мы») с мультиязычным контентом

Админ-панель с удобным интерфейсом, переключением языков и предпросмотром

Управление языками через админку (добавление / удаление с автоматическим созданием переведённых страниц)

Google Cloud Translation API для автоматического перевода

Архитектура
Проект построен по многослойной архитектуре, разделённой на зоны ответственности:

Models — сущности БД (Section, Page, ContentBlock, BlockStyle, Language)

Data — контекст AppDbContext (Entity Framework Core)

Interfaces — контракты сервисов (IBlockService, ISectionService, IPreviewService, ILanguageService, ITranslationService, ICaseTemplateBuilder)

Services — бизнес-логика (BlockService, SectionService, PreviewService, LanguageService, TranslationService, CaseTemplateBuilder)

Controllers — AdminController, CaseController, WhoWeAreController, HomeController

Views — Razor-представления (админка, публичные страницы)

Применяются принципы SOLID, внедрение зависимостей (DI), все операции с базой данных вынесены в сервисы.

Технологический стек
Backend: ASP.NET Core MVC (.NET 8+)

ORM: Entity Framework Core

База данных: Microsoft SQL Server

Переводы: Google Cloud Translation API v2

Аутентификация: (опционально) ASP.NET Core Identity (не включена в текущую версию)

Хранение изображений: локальная папка wwwroot/pictures

Контейнеризация: (опционально) Docker

Установка и запуск
Требования
.NET 8 SDK

SQL Server (LocalDB, Express или полноценный сервер)

Git

API-ключ Google Cloud Translation (для автоматического перевода)

Конфигурация
Клонируйте репозиторий:

bash
git clone https://github.com/MajorIndustry/DrozdovLaw.git
cd DrozdovLaw
Настройте строку подключения в appsettings.json:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DrozdovLawDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
Настройте Google Translate API (если планируете использовать авто-перевод):

Получите API-ключ в Google Cloud Console

Включите Cloud Translation API

Добавьте ключ в appsettings.json:

json
{
  "GoogleTranslate": {
    "ApiKey": "ВАШ_API_КЛЮЧ"
  }
}
Установите инструменты EF Core (если ещё не установлены):

bash
dotnet tool install --global dotnet-ef
Сборка и миграции
Восстановите пакеты:

bash
dotnet restore
Создайте миграцию и обновите базу данных:

bash
dotnet ef migrations add InitialCreate
dotnet ef database update
Запустите приложение:

bash
dotnet run
Откройте в браузере:

Публичная часть: https://localhost:5001

Админ-панель: https://localhost:5001/Admin

Использование
Админ-панель (/Admin)
Разделы (Cases)

Создание нового раздела с заполнением данных на базовом языке – остальные языки переводятся автоматически

Редактирование блоков (текст, стиль, цвет, изображения)

Изменение порядка блоков перетаскиванием

Копирование и перевод блоков с одного языка на другой

Редактирование метаданных раздела (название, статус, описание, флаг, цвет)

Статические страницы («Кто мы»)

Управление языками – добавление/удаление языков; при добавлении автоматически создаются страницы для всех разделов с переведённым контентом

Публичная часть
Список кейсов с карточками (название, статус, флаг, описание)

Детальная страница кейса с гибкой вёрсткой блоков

Страница «Кто мы»

Переключение языка в шапке и футере

Структура проекта
text
DrozdovLaw/
├── Controllers/
│   ├── AdminController.cs
│   ├── CaseController.cs
│   ├── HomeController.cs
│   └── WhoWeAreController.cs
├── Data/
│   └── AppDbContext.cs
├── Interfaces/
│   ├── IBlockService.cs
│   ├── ICaseTemplateBuilder.cs
│   ├── ILanguageService.cs
│   ├── IPreviewService.cs
│   ├── ISectionService.cs
│   └── ITranslationService.cs
├── Models/
│   ├── ViewModels/
│   │    ├── AdminEditViewModel.cs
│   │    ├── AdminIndexViewModel.cs
│   │    ├── CaseViewModel.cs
│   │    ├── CreateSectionViewModel.cs
│   │    ├── EditSectionViewModel.cs
│   │    ├── PageViewModel.cs
│   ├── BlockStyle.cs
│   ├── ContentBlock.cs
│   ├── Language.cs
│   ├── Page.cs
│   └── Section.cs
├── Services/
│   ├── BlockService.cs
│   ├── CaseTemplateBuilder.cs
│   ├── LanguageService.cs
│   ├── PreviewService.cs
│   ├── SectionService.cs
│   └── TranslationService.cs
├── Views/
│   ├── Admin/
│   │   ├── Index.cshtml
│   │   ├── EditBlock.cshtml
│   │   ├── EditSection.cshtml
│   │   ├── CreateSection.cshtml
│   │   └── ...
│   ├── Case/
│   │   ├── Detail.cshtml
│   │   └── List.cshtml
│   ├── WhoWeAre/
│   │   └── Index.cshtml
│   └── Shared/
│       ├── _Layout.cshtml
│       └── _ContentBlock.cshtml
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── pictures/
└── Program.cs
API и сервисы
Основные сервисы
Сервис	Описание
IBlockService / BlockService	CRUD для контент-блоков, работа со страницами, стилями, языками
ISectionService / SectionService	Управление разделами, создание с автопереводом, копирование блоков
ILanguageService / LanguageService	Управление языками, создание статических страниц при добавлении языка
IPreviewService / PreviewService	Генерация предпросмотра страниц для админки
ITranslationService / TranslationService	Интеграция с Google Cloud Translation API
API эндпоинты
Маршрут	Назначение
GET /Admin	Главная админ-панели
POST /Admin/CreateSection	Создание раздела
POST /Admin/EditBlock	Сохранение блока
POST /Admin/ReorderBlocks	Изменение порядка блоков
POST /Admin/UploadImage	Загрузка изображения
POST /Admin/CopyAndTranslateBlocks	Копирование и перевод блоков
GET /Case/Detail/{slug}	Детальная страница кейса
GET /Case/List	Список кейсов
