﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using System.IO;
using Windows.Storage;

namespace wikia_Unofficial.Models
{
    public class wikiaModels : DbContext
    {
        public DbSet<Wiki> Wikis { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databaseFilePath = "Wikki.db";

            try
            {
                databaseFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, databaseFilePath);
            }
            catch(InvalidOperationException)
            { }

            optionsBuilder.UseSqlite($"Data source={databaseFilePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wiki>().Property(b => b.Url).Required();
            modelBuilder.Entity<Wiki>().Property(b => b.WikiId).Required();
            modelBuilder.Entity<Wiki>().Property(b => b.WikiName).Required();
            modelBuilder.Entity<Wiki>().Property(b => b.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Favorite>().Property(b => b.Url).Required();
            modelBuilder.Entity<Favorite>().Property(b => b.FavoriteName).Required();
            modelBuilder.Entity<Favorite>().Property(b => b.WikiId).Required();
            modelBuilder.Entity<Favorite>().Property(b => b.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Setting>().Property(b => b.hasRun).Required();
            modelBuilder.Entity<Setting>().Property(b => b.Id).ValueGeneratedOnAdd();
        }
    }

    public class Wiki
    {
        public int WikiId { get; set; }
        public string Url { get; set; }
        public string WikiName { get; set; }
        public int Id { get; set; }
    }

    public class Favorite
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string FavoriteName { get; set; }
        public int WikiId { get; set; }
    }

    public class Setting
    {
        public bool hasRun { get; set; }
        public int Id { get; set; }
    }
}
