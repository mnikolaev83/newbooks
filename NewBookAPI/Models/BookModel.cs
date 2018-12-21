using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBookAPI.Models
{
    public class BookModel
    {
        public int id { get; set; }
        public int category_id { get; set; }
        public string category_name { get; set; }
        public string isbn { get; set; }
        public string store_code { get; set; }
        public string author_name { get; set; }
        public string title { get; set; }
        public string series { get; set; }
        public string subcategory { get; set; }
        public string description { get; set; }
        public string updated_at { get; set; }
        public string added_at { get; set; }
        public string image_url { get; set; }
        public string publisher { get; set; }
        public int year { get; set; }
        public int pages_amnt { get; set; }
        public int run_amnt { get; set; }
        public string target { get; set; }
        public string translated { get; set; }
        public bool is_ignored { get; set; }
        public bool is_favorite { get; set; }
        public bool is_in_wishlist { get; set; }

    }
}
