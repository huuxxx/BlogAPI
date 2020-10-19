using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class BlogItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Secret { get; set; }
    }
}
