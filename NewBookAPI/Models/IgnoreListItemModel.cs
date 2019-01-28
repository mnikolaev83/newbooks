using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBookAPI.Models
{
    public class IgnoreListItemModel
    {
        public int id { get; set; }
        public string category_name { get; set; }
        public string series { get; set; }
        public string subcategory { get; set; }
        public string target { get; set; }
        public string publisher { get; set; }
    }
}
