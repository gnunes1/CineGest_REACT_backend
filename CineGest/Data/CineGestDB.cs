using CineGest.Models;
using Microsoft.EntityFrameworkCore;

namespace CineGest.Data
{
    /// <summary>
    /// classe que representa a BD
    /// </summary>
    public class CineGestDB : DbContext
    {
        /// <summary>
        /// construtor da classe
        /// relaciona a classe à DB
        /// </summary>
        /// <param name="options"></param>
        public CineGestDB(DbContextOptions<CineGestDB> options) : base(options)
        {
        }

        public DbSet<Cinemas> Cinema { get; set; }

        public DbSet<Sessions> Cinema_Movie { get; set; }

        public DbSet<Movies> Movie { get; set; }

        public DbSet<Sessions> Room_Movie { get; set; }

        public DbSet<Tickets> Ticket { get; set; }

        public DbSet<Users> User { get; set; }

        public DbSet<Roles> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Users>()
                 .HasIndex(u => u.Email)
                 .IsUnique();
            builder.Entity<Movies>()
                .HasIndex(m => m.Name)
                .IsUnique();
            builder.Entity<Cinemas>()
                .HasIndex(c => c.Name)
                .IsUnique();

            builder.Entity<Roles>().HasData(new { Id = 1, Name = "Admin" }, new { Id = 2, Name = "User" });
        }
    }
}
