using ApiRocketMovies.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiRocketMovies.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Renomeia a tabela padrão do Identity para "users"
            modelBuilder.Entity<User>().ToTable("users");

            // Configuração do User
            modelBuilder.Entity<User>(u =>
            {
                u.HasKey(u => u.Id);
                u.Property(u => u.Id).ValueGeneratedOnAdd();
                u.Property(u => u.Name).IsRequired().HasMaxLength(100);
                u.Property(u => u.Avatar).HasMaxLength(255);
                u.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
                u.Property(u => u.UpdatedAt).HasDefaultValueSql("GETDATE()");

                // Relacionamento 1:N User -> Movies
                u.HasMany(u => u.Movies)
                 .WithOne(m => m.User)
                 .HasForeignKey(m => m.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Relacionamento 1:N User -> Tags
                u.HasMany(u => u.Tags)
                 .WithOne(t => t.User)
                 .HasForeignKey(t => t.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuração do Movie
            modelBuilder.Entity<Movie>(m =>
            {
                m.ToTable("movies");
                m.HasKey(m => m.Id);
                m.Property(m => m.Id).ValueGeneratedOnAdd();
                m.Property(m => m.Title).IsRequired().HasMaxLength(100);
                m.Property(m => m.Description).IsRequired().HasMaxLength(500);
                m.Property(m => m.Rating).IsRequired();
                m.Property(m => m.UserId).IsRequired().HasMaxLength(450);
                m.Property(m => m.CreatedAt).HasDefaultValueSql("GETDATE()");
                m.Property(m => m.UpdatedAt).HasDefaultValueSql("GETDATE()");

                // Relacionamento 1:N Movie -> Tags
                m.HasMany(m => m.Tags)
                 .WithOne(t => t.Movie)
                 .HasForeignKey(t => t.MovieId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração do Tag
            modelBuilder.Entity<Tag>(t =>
            {
                t.ToTable("tags");
                t.HasKey(t => t.Id);
                t.Property(t => t.Id).ValueGeneratedOnAdd();
                t.Property(t => t.MovieId).IsRequired();
                t.Property(t => t.UserId).IsRequired().HasMaxLength(450);
                t.Property(t => t.Name).IsRequired().HasMaxLength(100);
            });
        }
    }
}
