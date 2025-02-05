using ApiRocketMovies.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiRocketMovies.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(u =>
            {
                u.ToTable("users");
                u.HasKey(u => u.Id);
                u.Property(u => u.Id).ValueGeneratedOnAdd();
                u.Property(u => u.Name).IsRequired().HasMaxLength(100);
                u.Property(u => u.Email).IsRequired().HasMaxLength(100);
                u.Property(u => u.Password).IsRequired().HasMaxLength(8);
                u.Property(u => u.Avatar).HasMaxLength(255);
                u.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
                u.Property(u => u.UpdatedAt).HasDefaultValueSql("GETDATE()");

                // Relacionamento 1:N User -> Movies
                u.HasMany(m => m.Movies)
                 .WithOne(u => u.User)
                 .HasForeignKey(m => m.UserId)
                 .OnDelete(DeleteBehavior.Restrict); // Evitar cascata
            });
            
            modelBuilder.Entity<Movie>(m =>
            {
                m.ToTable("movies");
                m.HasKey(m => m.Id);
                m.Property(m => m.Id).ValueGeneratedOnAdd();
                m.Property(m => m.Title).IsRequired().HasMaxLength(100);
                m.Property(m => m.Description).IsRequired().HasMaxLength(500);
                m.Property(m => m.Rating).IsRequired();
                m.Property(m => m.UserId).IsRequired();
                m.Property(m => m.CreatedAt).HasDefaultValueSql("GETDATE()");
                m.Property(m => m.UpdatedAt).HasDefaultValueSql("GETDATE()");

                // Relacionamento 1:N Movie -> Tags
                m.HasMany(t => t.Tags)
                 .WithOne(m => m.Movie)
                 .HasForeignKey(t => t.MovieId)
                 .OnDelete(DeleteBehavior.Cascade); // Cascata permitida aqui
            });

            modelBuilder.Entity<Tag>(t =>
            {
                t.ToTable("tags");
                t.HasKey(t => t.Id);
                t.Property(t => t.Id).ValueGeneratedOnAdd();
                t.Property(t => t.MovieId).IsRequired();
                t.Property(t => t.UserId).IsRequired();
                t.Property(t => t.Name).IsRequired().HasMaxLength(100);

                // Relacionamento 1:N Tag -> User
                t.HasOne(u => u.User)
                 .WithMany(t => t.Tags)
                 .HasForeignKey(t => t.UserId)
                 .OnDelete(DeleteBehavior.Restrict); // Evitar cascata
            });
        }

    }
}

