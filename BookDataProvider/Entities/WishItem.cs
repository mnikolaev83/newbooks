using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BookDataProvider.Entities
{
    public class WishItem
    {
        public int Id { get; set; }
        public Book Book { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}