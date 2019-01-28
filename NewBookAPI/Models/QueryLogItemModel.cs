using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBookAPI.Models
{
    public class QueryLogItemModel
    {
        public int id { get; set; }
        public string query_at { get; set; }
        public string period { get; set; }
        public string category_name { get; set; }
        public int books_fetched { get; set; }
    }
}
