using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BookDataProvider.Entities
{
    public class QueryLog
    {
        public int Id { get; set; }
        public DateTime QueryAt { get; set; } = DateTime.Now;
        public Category Category { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int BooksFetched { get; set; }

    }
}