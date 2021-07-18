using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class EditBlog
    {
        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
    }
}
