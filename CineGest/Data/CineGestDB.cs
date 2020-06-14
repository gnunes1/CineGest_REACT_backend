using CineGest.Models;
using Microsoft.EntityFrameworkCore;
using System;

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

            builder.Entity<Roles>().HasData(
                new { Id = 1, Name = "Admin" },
                new { Id = 2, Name = "User" }
                );

            builder.Entity<Users>().HasData(new
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@admin",
                RoleFK = 1,
                DoB = DateTime.UtcNow,
                TokenCreatedAt = new DateTime(),
                Hash = "8C-69-76-E5-B5-41-04-15-BD-E9-08-BD-4D-EE-15-DF-B1-67-A9-C8-73-FC-4B-B8-A8-1F-6F-2A-B4-48-A9-18"
            });
        }
    }
}
