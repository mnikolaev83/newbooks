using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BookDataProvider.Entities;
using Microsoft.EntityFrameworkCore;
namespace BookDataProvider
{
    public class BookDBContext : DbContext
    {
        private static string ConnectionString { get; set; } = String.Empty;
        public BookDBContext(DbContextOptions options) : base(options)
        {
        }
        public BookDBContext() : base()
        {
        }
        public static void SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<IgnoreItem> IgnoreList { get; set; }
        public DbSet<FavoriteItem> FavoriteList { get; set; }
        public DbSet<WishItem> WishList { get; set; }
        public DbSet<JobLog> JobLog { get; set; }
        public DbSet<QueryLog> QueryLog { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
