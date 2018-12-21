using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BookDataProvider.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public string Isbn { get; set; } = String.Empty;
        public string StoreCode { get; set; } = String.Empty;
        public string AuthorShortName { get; set; } = String.Empty;
        public string Title { get; set; } = String.Empty;
        public string Series { get; set; } = String.Empty;
        public string Subcategory { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime AddedAt { get; set; } = DateTime.Now;
        public string AuthorFullName { get; set; } = String.Empty;
        public DateTime DateAddedToStore { get; set; }
        public string ImageURL { get; set; } = String.Empty;
        public string Publisher { get; set; } = String.Empty;
        public int Year { get; set; }
        public int PagesAmnt { get; set; }
        public int RunAmnt { get; set; }
        public string Target { get; set; } = String.Empty;
        public string Translated { get; set; } = String.Empty;

    }
}