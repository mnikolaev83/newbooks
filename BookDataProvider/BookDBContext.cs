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
        public BookDBContext(DbContextOptions options) : base(options)
        {
        }
        public BookDBContext() : base()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-KFK74RP\HOMEPC;Initial Catalog=NewBooks;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
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
