using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Data
{
    public class CulturalHeritageDbContext : DbContext
    {
        public CulturalHeritageDbContext(DbContextOptions<CulturalHeritageDbContext> options)
            : base(options)
        {
        }

        public DbSet<Heritage> Heritage { get; set; }
        public DbSet<NationalMinority> NationalMinority { get; set; }
        public DbSet<Theme> Theme { get; set; }
        public DbSet<HeritageTheme> HeritageTheme { get; set; }
        public DbSet<User> User { get; set; }
       
        public DbSet<UserHeritageComment> UserHeritageComment { get; set; }
        public DbSet<Log> Log { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User"); // treba specificirat iz nekog razloga??
            modelBuilder.Entity<Heritage>()
                .HasMany(h => h.HeritageTheme)
                .WithOne(ht => ht.Heritage)
                .HasForeignKey(ht => ht.HeritageId)
                .OnDelete(DeleteBehavior.Cascade); // ksakadno brisanje za HeritageThemes

            modelBuilder.Entity<Heritage>()
                .HasMany(h => h.UserHeritageComment)
                .WithOne(uhc => uhc.Heritage)
                .HasForeignKey(uhc => uhc.HeritageId)
                .OnDelete(DeleteBehavior.Cascade); // kaskadno brisanje za UserHeritageComments


            // Configure composite key for HeritageTheme (M:N bridge table)
            modelBuilder.Entity<HeritageTheme>()
                .HasKey(ht => new { ht.HeritageId, ht.ThemeId });

            modelBuilder.Entity<HeritageTheme>()
                .HasOne(ht => ht.Heritage)
                .WithMany(h => h.HeritageTheme)
                .HasForeignKey(ht => ht.HeritageId);

            modelBuilder.Entity<HeritageTheme>()
                .HasOne(ht => ht.Theme)
                .WithMany(t => t.HeritageThemes)
                .HasForeignKey(ht => ht.ThemeId);

            // Configure composite key for UserHeritageComment
            modelBuilder.Entity<UserHeritageComment>()
                .HasKey(uhc => new { uhc.UserId, uhc.HeritageId, uhc.CreatedAt });

            modelBuilder.Entity<UserHeritageComment>()
                .HasOne(uhc => uhc.User)
                .WithMany(u => u.UserHeritageComments)
                .HasForeignKey(uhc => uhc.UserId);

            modelBuilder.Entity<UserHeritageComment>()
                .HasOne(uhc => uhc.Heritage)
                .WithMany(h => h.UserHeritageComment)
                .HasForeignKey(uhc => uhc.HeritageId);
        }
    }
}
