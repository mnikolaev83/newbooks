using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BookDataProvider.Entities
{
    public class FavoriteItem
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public string Series { get; set; } = String.Empty;
        public string Subcategory { get; set; } = String.Empty;
        public string Target { get; set; } = String.Empty;
        public string Publisher { get; set; } = String.Empty;
    }
}