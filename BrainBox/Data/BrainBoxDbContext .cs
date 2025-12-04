using Microsoft.EntityFrameworkCore;
using BrainBox.Models;

namespace BrainBox.Data
{
    public class BrainBoxDbContext : DbContext
    {
        public BrainBoxDbContext(DbContextOptions<BrainBoxDbContext> options)
            : base(options)
        {
        }

        public DbSet<Idea> Ideas { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<IdeaTheme> IdeaThemes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // MM configuration
            modelBuilder.Entity<IdeaTheme>()
                .HasKey(it => new { it.IdeaId, it.ThemeId });

            modelBuilder.Entity<IdeaTheme>()
                .HasOne(it => it.Idea)
                .WithMany(i => i.IdeaThemes)
                .HasForeignKey(it => it.IdeaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IdeaTheme>()
                .HasOne(it => it.Theme)
                .WithMany(t => t.IdeaThemes)
                .HasForeignKey(it => it.ThemeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Idea>()
                .HasIndex(i => i.CreatedAt);

            modelBuilder.Entity<Idea>()
                .HasIndex(i => i.LastModifiedAt);
        }
    }
}
