using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBookAPI.Models
{
    public class NewBooksLogItemModel
    {
        public int id { get; set; }
        public string started_at { get; set; }
        public string completed_at { get; set; }
        public int books_fetched { get; set; }
        public bool error_occured { get; set; }

    }
}
