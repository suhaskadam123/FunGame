
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.Reflection.Emit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataAccessLayer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {






        }

        public DbSet<User> Users { get; set; }
        public DbSet<BetData> BetData { get; set; }
        public DbSet<GameSetting> GameSetting { get; set; }
        public DbSet<WinningNumber> WinningNumber { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Balance).HasDefaultValue(0);
                entity.Property(e => e.Status).HasDefaultValue(0);
                entity.Property(e => e.SuperDistributerId).HasColumnName("SDId").HasDefaultValue(0);
                entity.Property(e => e.DistributerId).HasColumnName("DId").HasDefaultValue(0);
                entity.Property(e => e.RetailerId).HasColumnName("RId").HasDefaultValue(0);
                entity.Property(e => e.SuperAdminId).HasColumnName("SId").HasDefaultValue(1);
                entity.Property(e => e.DeleteStatus).HasDefaultValue(1);
                entity.Property(e => e.Note).IsRequired().HasMaxLength(255);
                entity.Property(e => e.UniqueId).IsRequired().HasMaxLength(6);
            });

            // Seed initial superadmin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "superadmin",
                    Password = "admin123",
                    Balance = int.MaxValue,//2147483647
                    Role = "superadmin",
                    Status = 1,
                    SuperAdminId = 1,
                    DeleteStatus = 1,
                    Note = "Initial SuperAdmin User",
                    Percentage = 0,
                    DateTime = DateTime.Now,
                    ReferName = "superadmin",
                    UniqueId = "123456"
                }
            );
        }
    }
}
//Add - Migration - Name "InitialCreate" - Project "DataAccessLayer" - StartupProject "game_admin"
//Update - Database - Project "DataAccessLayer" - StartupProject "game_admin"




