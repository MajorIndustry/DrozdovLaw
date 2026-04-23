using DrozdovLaw.Models;
using Microsoft.EntityFrameworkCore;

namespace DrozdovLaw.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<BlockStyle> BlockStyles => Set<BlockStyle>();
    public DbSet<ContentBlock> ContentBlocks => Set<ContentBlock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Language ---
        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.Code);
            entity.HasData(
                new Language { Code = "ru", Name = "Русский" },
                new Language { Code = "en", Name = "English" }
            );
        });

        // --- Section ---
        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasIndex(e => e.Slug)
                  .IsUnique()
                  .HasDatabaseName("UQ_Sections_Slug");
        });

        // --- Page ---
        modelBuilder.Entity<Page>(entity =>
        {
            entity.HasIndex(e => new { e.SystemName, e.LanguageCode, e.SectionId })
        .IsUnique()
        .HasDatabaseName("UQ_Pages_SystemName_LanguageCode_SectionId");
            entity.HasOne(p => p.Language)
                  .WithMany(l => l.Pages)
                  .HasForeignKey(p => p.LanguageCode)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Section)
                  .WithMany(s => s.Pages)
                  .HasForeignKey(p => p.SectionId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Предустановленные статические страницы
            entity.HasData(
                new Page { Id = 1, SystemName = "case", LanguageCode = "ru", Name = "Кейсы" },
                new Page { Id = 2, SystemName = "case", LanguageCode = "en", Name = "Cases" }
                //new Page { Id = 3, SystemName = "whoweare", Name = "Кто мы", LanguageCode = "ru" },
                //new Page { Id = 4, SystemName = "whoweare", Name = "Who we are", LanguageCode = "en" }
            );
        });

        // --- BlockStyle ---
        modelBuilder.Entity<BlockStyle>(entity =>
        {
            entity.HasIndex(e => e.Name)
                  .IsUnique()
                  .HasDatabaseName("UQ_BlockStyles_Name");

            entity.HasData(
                new BlockStyle { Id = 1, Name = "h1", Description = "Заголовок 1-го уровня" },
                new BlockStyle { Id = 2, Name = "h2", Description = "Заголовок 2-го уровня" },
                new BlockStyle { Id = 3, Name = "h3", Description = "Заголовок 3-го уровня" },
                new BlockStyle { Id = 4, Name = "h4", Description = "Заголовок 4-го уровня" },
                new BlockStyle { Id = 5, Name = "h5", Description = "Заголовок 5-го уровня" },
                new BlockStyle { Id = 6, Name = "p", Description = "Обычный абзац" },
                new BlockStyle { Id = 7, Name = "p-large", Description = "Крупный абзац" },
                new BlockStyle { Id = 8, Name = "blockquote", Description = "Цитата" },
                new BlockStyle { Id = 9, Name = "small", Description = "Мелкий текст / сноска" },
                new BlockStyle { Id = 10, Name = "ul", Description = "Маркированный список (через |)" },
                new BlockStyle { Id = 11, Name = "ol", Description = "Нумерованный список (через |)" },
                new BlockStyle { Id = 12, Name = "e-id", Description = "Номер дела" },
                new BlockStyle { Id = 13, Name = "e-details__type", Description = "Тип результата" },
                new BlockStyle { Id = 14, Name = "e-details__loc", Description = "Локация / юрисдикция" },
                new BlockStyle { Id = 15, Name = "e-details__def", Description = "Определение / тип дела" },
                new BlockStyle { Id = 16, Name = "note-title", Description = "Заголовок пояснения" },
                new BlockStyle { Id = 17, Name = "note-text", Description = "Текст пояснения" },
                new BlockStyle { Id = 18, Name = "breadcrumb", Description = "Хлебные крошки (через /)" },
                new BlockStyle { Id = 19, Name = "section-title", Description = "Заголовок секции" },
                new BlockStyle { Id = 20, Name = "e-dots", Description = "Цветные точки (цвета через ,)" },
                new BlockStyle { Id = 21, Name = "person", Description = "Адвокат" },
                new BlockStyle { Id = 22, Name = "decision-title", Description = "Заголовок 'Решение'" },
                new BlockStyle { Id = 23, Name = "decision-text", Description = "Текст решения" },
                new BlockStyle { Id = 24, Name = "case-link", Description = "Ссылка на дело" },
                new BlockStyle { Id = 25, Name = "docs-title", Description = "Заголовок 'Документы по делу'" },
                new BlockStyle { Id = 26, Name = "doc-item", Description = "Документ PDF" },
                new BlockStyle { Id = 27, Name = "tags-title", Description = "Заголовок 'Теги'" },
                new BlockStyle { Id = 28, Name = "tag", Description = "Тег" }
            );
        });

        // --- ContentBlock ---
        modelBuilder.Entity<ContentBlock>(entity =>
        {
            entity.HasIndex(e => new { e.PageId, e.Order })
                  .HasDatabaseName("IX_ContentBlocks_PageId_Order");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(b => b.Page)
                  .WithMany(p => p.ContentBlocks)
                  .HasForeignKey(b => b.PageId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.Style)
                  .WithMany(s => s.ContentBlocks)
                  .HasForeignKey(b => b.StyleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}